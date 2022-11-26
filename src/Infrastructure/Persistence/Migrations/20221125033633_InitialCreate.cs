using System;
using System.Net;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Smilodon.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        private const string AccountsSearchIndex = """
        CREATE INDEX search_index
            ON accounts USING gin (((setweight(to_tsvector('simple'::regconfig, display_name::text), 'A'::"char") ||
                                     setweight(to_tsvector('simple'::regconfig, username::text), 'B'::"char")) ||
                                    setweight(to_tsvector('simple'::regconfig, COALESCE(domain, ''::CHARACTER VARYING)::text),
                                              'C'::"char")));
        """;
        
        private const string AccountsUsernameAndDomainLowerIndex = """
        CREATE UNIQUE INDEX index_accounts_on_username_and_domain_lower
            ON accounts (lower(username::text), COALESCE(lower(domain::text), ''::text));
        """;
        
        private const string TagsNameLowerBtreeIndex = """
        CREATE UNIQUE INDEX index_tags_on_name_lower_btree
            ON tags (lower(name::text) text_pattern_ops);
        """;
        
        private const string TimestampIdFunction = """
        CREATE FUNCTION timestamp_id(table_name text) RETURNS bigint
            LANGUAGE plpgsql
        AS
        $$
          DECLARE
            time_part bigint;
            sequence_base bigint;
            tail bigint;
          BEGIN
            time_part := (
              -- Get the time in milliseconds
              ((date_part('epoch', now()) * 1000))::bigint
              -- And shift it over two bytes
              << 16);

            sequence_base := (
              'x' ||
              -- Take the first two bytes (four hex characters)
              substr(
                -- Of the MD5 hash of the data we documented
                md5(table_name || '34bfaf6f0285d60227c6a18d28dab3f2' || time_part::text),
                1, 4
              )
            -- And turn it into a bigint
            )::bit(16)::bigint;

            -- Finally, add our sequence number to our base, and chop
            -- it to the last two bytes
            tail := (
              (sequence_base + nextval(table_name || '_id_seq'))
              & 65535);

            -- Return the time part and the sequence part. OR appears
            -- faster here than addition, but they're equivalent:
            -- time_part has no trailing two bytes, and tail is only
            -- the last two bytes.
            RETURN time_part | tail;
          END
        $$;
        """;
        
        private const string UserIpsView = """
        CREATE VIEW user_ips(user_id, ip, used_at) AS
            SELECT t0.user_id,
                   t0.ip,
                   max(t0.used_at) AS used_at
            FROM (SELECT users.id         AS user_id,
                         users.sign_up_ip AS ip,
                         users.created_at AS used_at
                  FROM users
                  WHERE users.sign_up_ip IS NOT NULL
                  UNION ALL
                  SELECT session_activations.user_id,
                         session_activations.ip,
                         session_activations.updated_at
                  FROM session_activations
                  UNION ALL
                  SELECT login_activities.user_id,
                         login_activities.ip,
                         login_activities.created_at
                  FROM login_activities
                  WHERE login_activities.success = true) t0
            GROUP BY t0.user_id, t0.ip;
        """;
        
        private const string AccountSummariesView = """
        CREATE MATERIALIZED VIEW account_summaries AS
            SELECT accounts.id                                 AS account_id,
                   mode() WITHIN GROUP (ORDER BY t0.language)  AS language,
                   mode() WITHIN GROUP (ORDER BY t0.sensitive) AS sensitive
            FROM accounts
                     CROSS JOIN LATERAL ( SELECT statuses.account_id,
                                                 statuses.language,
                                                 statuses.sensitive
                                          FROM statuses
                                          WHERE statuses.account_id = accounts.id
                                            AND statuses.deleted_at IS NULL
                                            AND statuses.reblog_of_id IS NULL
                                          ORDER BY statuses.id DESC
                                          LIMIT 20) t0
            WHERE accounts.suspended_at IS NULL
              AND accounts.silenced_at IS NULL
              AND accounts.moved_to_account_id IS NULL
              AND accounts.discoverable = true
              AND accounts.locked = false
            GROUP BY accounts.id;
        
        CREATE UNIQUE INDEX index_account_summaries_on_account_id
            ON account_summaries (account_id);
        """;
        
        private const string FollowRecommendationsView = """
        CREATE MATERIALIZED VIEW public.follow_recommendations AS
            SELECT t0.account_id,
                   sum(t0.rank)         AS rank,
                   array_agg(t0.reason) AS reason
            FROM (SELECT account_summaries.account_id,
                         count(follows.id)::numeric / (1.0 + count(follows.id)::numeric) AS rank,
                         'most_followed'::text                                           AS reason
                  FROM follows
                           JOIN account_summaries ON account_summaries.account_id = follows.target_account_id
                           JOIN users ON users.account_id = follows.account_id
                           LEFT JOIN follow_recommendation_suppressions
                                     ON follow_recommendation_suppressions.account_id = follows.target_account_id
                  WHERE users.current_sign_in_at >= (now() - '30 days'::interval)
                    AND account_summaries.sensitive = false
                    AND follow_recommendation_suppressions.id IS NULL
                  GROUP BY account_summaries.account_id
                  HAVING count(follows.id) >= 5
                  UNION ALL
                  SELECT account_summaries.account_id,
                         sum(status_stats.reblogs_count + status_stats.favourites_count) /
                         (1.0 + sum(status_stats.reblogs_count + status_stats.favourites_count)) AS rank,
                         'most_interactions'::text                                               AS reason
                  FROM status_stats
                           JOIN statuses ON statuses.id = status_stats.status_id
                           JOIN account_summaries ON account_summaries.account_id = statuses.account_id
                           LEFT JOIN follow_recommendation_suppressions
                                     ON follow_recommendation_suppressions.account_id = statuses.account_id
                  WHERE statuses.id >=
                        ((date_part('epoch'::text, now() - '30 days'::interval) * 1000::double precision)::bigint << 16)
                    AND account_summaries.sensitive = false
                    AND follow_recommendation_suppressions.id IS NULL
                  GROUP BY account_summaries.account_id
                  HAVING sum(status_stats.reblogs_count + status_stats.favourites_count) >= 5::numeric) t0
            GROUP BY t0.account_id
            ORDER BY (sum(t0.rank)) DESC;
        
        CREATE UNIQUE INDEX index_follow_recommendations_on_account_id
            ON follow_recommendations (account_id); 
        """;
        
        private const string InstancesView = """
        CREATE MATERIALIZED VIEW public.instances AS
            WITH domain_counts(domain, accounts_count) AS (SELECT accounts.domain,
                                                                  count(*) AS accounts_count
                                                           FROM accounts
                                                           WHERE accounts.domain IS NOT NULL
                                                           GROUP BY accounts.domain)
            SELECT domain_counts.domain,
                   domain_counts.accounts_count
            FROM domain_counts
            UNION
            SELECT domain_blocks.domain,
                   COALESCE(domain_counts.accounts_count, 0::bigint) AS accounts_count
            FROM domain_blocks
                     LEFT JOIN domain_counts ON domain_counts.domain::text = domain_blocks.domain::text
            UNION
            SELECT domain_allows.domain,
                   COALESCE(domain_counts.accounts_count, 0::bigint) AS accounts_count
            FROM domain_allows
                     LEFT JOIN domain_counts ON domain_counts.domain::text = domain_allows.domain::text; 

        CREATE UNIQUE INDEX index_instances_on_domain
            ON public.instances (domain); 
        """;
        
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:plpgsql", ",,");

            migrationBuilder.Sql(TimestampIdFunction);

            migrationBuilder.CreateSequence(
                name: "accounts_id_seq");

            migrationBuilder.CreateSequence(
                name: "encrypted_messages_id_seq");

            migrationBuilder.CreateSequence(
                name: "media_attachments_id_seq");

            migrationBuilder.CreateSequence(
                name: "statuses_id_seq");

            migrationBuilder.CreateTable(
                name: "account_warning_presets",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    text = table.Column<string>(type: "text", nullable: false, defaultValueSql: "''::text"),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false),
                    title = table.Column<string>(type: "character varying", nullable: false, defaultValueSql: "''::character varying")
                },
                constraints: table =>
                {
                    table.PrimaryKey("account_warning_presets_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "accounts",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "timestamp_id('accounts'::text)"),
                    username = table.Column<string>(type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    domain = table.Column<string>(type: "character varying", nullable: true),
                    privatekey = table.Column<string>(name: "private_key", type: "text", nullable: true),
                    publickey = table.Column<string>(name: "public_key", type: "text", nullable: false, defaultValueSql: "''::text"),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false),
                    note = table.Column<string>(type: "text", nullable: false, defaultValueSql: "''::text"),
                    displayname = table.Column<string>(name: "display_name", type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    uri = table.Column<string>(type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    url = table.Column<string>(type: "character varying", nullable: true),
                    avatarfilename = table.Column<string>(name: "avatar_file_name", type: "character varying", nullable: true),
                    avatarcontenttype = table.Column<string>(name: "avatar_content_type", type: "character varying", nullable: true),
                    avatarfilesize = table.Column<int>(name: "avatar_file_size", type: "integer", nullable: true),
                    avatarupdatedat = table.Column<DateTime>(name: "avatar_updated_at", type: "timestamp without time zone", nullable: true),
                    headerfilename = table.Column<string>(name: "header_file_name", type: "character varying", nullable: true),
                    headercontenttype = table.Column<string>(name: "header_content_type", type: "character varying", nullable: true),
                    headerfilesize = table.Column<int>(name: "header_file_size", type: "integer", nullable: true),
                    headerupdatedat = table.Column<DateTime>(name: "header_updated_at", type: "timestamp without time zone", nullable: true),
                    avatarremoteurl = table.Column<string>(name: "avatar_remote_url", type: "character varying", nullable: true),
                    locked = table.Column<bool>(type: "boolean", nullable: false),
                    headerremoteurl = table.Column<string>(name: "header_remote_url", type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    lastwebfingeredat = table.Column<DateTime>(name: "last_webfingered_at", type: "timestamp without time zone", nullable: true),
                    inboxurl = table.Column<string>(name: "inbox_url", type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    outboxurl = table.Column<string>(name: "outbox_url", type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    sharedinboxurl = table.Column<string>(name: "shared_inbox_url", type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    followersurl = table.Column<string>(name: "followers_url", type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    protocol = table.Column<int>(type: "integer", nullable: false),
                    memorial = table.Column<bool>(type: "boolean", nullable: false),
                    movedtoaccountid = table.Column<long>(name: "moved_to_account_id", type: "bigint", nullable: true),
                    featuredcollectionurl = table.Column<string>(name: "featured_collection_url", type: "character varying", nullable: true),
                    fields = table.Column<string>(type: "jsonb", nullable: true),
                    actortype = table.Column<string>(name: "actor_type", type: "character varying", nullable: true),
                    discoverable = table.Column<bool>(type: "boolean", nullable: true),
                    alsoknownas = table.Column<string[]>(name: "also_known_as", type: "character varying[]", nullable: true),
                    silencedat = table.Column<DateTime>(name: "silenced_at", type: "timestamp without time zone", nullable: true),
                    suspendedat = table.Column<DateTime>(name: "suspended_at", type: "timestamp without time zone", nullable: true),
                    hidecollections = table.Column<bool>(name: "hide_collections", type: "boolean", nullable: true),
                    avatarstorageschemaversion = table.Column<int>(name: "avatar_storage_schema_version", type: "integer", nullable: true),
                    headerstorageschemaversion = table.Column<int>(name: "header_storage_schema_version", type: "integer", nullable: true),
                    devicesurl = table.Column<string>(name: "devices_url", type: "character varying", nullable: true),
                    suspensionorigin = table.Column<int>(name: "suspension_origin", type: "integer", nullable: true),
                    sensitizedat = table.Column<DateTime>(name: "sensitized_at", type: "timestamp without time zone", nullable: true),
                    trendable = table.Column<bool>(type: "boolean", nullable: true),
                    reviewedat = table.Column<DateTime>(name: "reviewed_at", type: "timestamp without time zone", nullable: true),
                    requestedreviewat = table.Column<DateTime>(name: "requested_review_at", type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("accounts_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_2320833084",
                        column: x => x.movedtoaccountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "accounts_tags",
                columns: table => new
                {
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: false),
                    tagid = table.Column<long>(name: "tag_id", type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "announcements",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    text = table.Column<string>(type: "text", nullable: false, defaultValueSql: "''::text"),
                    published = table.Column<bool>(type: "boolean", nullable: false),
                    allday = table.Column<bool>(name: "all_day", type: "boolean", nullable: false),
                    scheduledat = table.Column<DateTime>(name: "scheduled_at", type: "timestamp without time zone", nullable: true),
                    startsat = table.Column<DateTime>(name: "starts_at", type: "timestamp without time zone", nullable: true),
                    endsat = table.Column<DateTime>(name: "ends_at", type: "timestamp without time zone", nullable: true),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false),
                    publishedat = table.Column<DateTime>(name: "published_at", type: "timestamp without time zone", nullable: true),
                    statusids = table.Column<long[]>(name: "status_ids", type: "bigint[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("announcements_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ar_internal_metadata",
                columns: table => new
                {
                    key = table.Column<string>(type: "character varying", nullable: false),
                    value = table.Column<string>(type: "character varying", nullable: true),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp(6) without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp(6) without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ar_internal_metadata_pkey", x => x.key);
                });

            migrationBuilder.CreateTable(
                name: "conversations",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    uri = table.Column<string>(type: "character varying", nullable: true),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("conversations_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "custom_emoji_categories",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying", nullable: true),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("custom_emoji_categories_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "custom_emojis",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    shortcode = table.Column<string>(type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    domain = table.Column<string>(type: "character varying", nullable: true),
                    imagefilename = table.Column<string>(name: "image_file_name", type: "character varying", nullable: true),
                    imagecontenttype = table.Column<string>(name: "image_content_type", type: "character varying", nullable: true),
                    imagefilesize = table.Column<int>(name: "image_file_size", type: "integer", nullable: true),
                    imageupdatedat = table.Column<DateTime>(name: "image_updated_at", type: "timestamp without time zone", nullable: true),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false),
                    disabled = table.Column<bool>(type: "boolean", nullable: false),
                    uri = table.Column<string>(type: "character varying", nullable: true),
                    imageremoteurl = table.Column<string>(name: "image_remote_url", type: "character varying", nullable: true),
                    visibleinpicker = table.Column<bool>(name: "visible_in_picker", type: "boolean", nullable: false, defaultValueSql: "true"),
                    categoryid = table.Column<long>(name: "category_id", type: "bigint", nullable: true),
                    imagestorageschemaversion = table.Column<int>(name: "image_storage_schema_version", type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("custom_emojis_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "domain_allows",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    domain = table.Column<string>(type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("domain_allows_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "domain_blocks",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    domain = table.Column<string>(type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false),
                    severity = table.Column<int>(type: "integer", nullable: true, defaultValueSql: "0"),
                    rejectmedia = table.Column<bool>(name: "reject_media", type: "boolean", nullable: false),
                    rejectreports = table.Column<bool>(name: "reject_reports", type: "boolean", nullable: false),
                    privatecomment = table.Column<string>(name: "private_comment", type: "text", nullable: true),
                    publiccomment = table.Column<string>(name: "public_comment", type: "text", nullable: true),
                    obfuscate = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("domain_blocks_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "email_domain_blocks",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    domain = table.Column<string>(type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false),
                    parentid = table.Column<long>(name: "parent_id", type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("email_domain_blocks_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_408efe0a15",
                        column: x => x.parentid,
                        principalTable: "email_domain_blocks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ip_blocks",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false),
                    expiresat = table.Column<DateTime>(name: "expires_at", type: "timestamp without time zone", nullable: true),
                    ip = table.Column<IPAddress>(type: "inet", nullable: false, defaultValueSql: "'0.0.0.0'::inet"),
                    severity = table.Column<int>(type: "integer", nullable: false),
                    comment = table.Column<string>(type: "text", nullable: false, defaultValueSql: "''::text")
                },
                constraints: table =>
                {
                    table.PrimaryKey("ip_blocks_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "pghero_space_stats",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    database = table.Column<string>(type: "text", nullable: true),
                    schema = table.Column<string>(type: "text", nullable: true),
                    relation = table.Column<string>(type: "text", nullable: true),
                    size = table.Column<long>(type: "bigint", nullable: true),
                    capturedat = table.Column<DateTime>(name: "captured_at", type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pghero_space_stats_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "preview_card_providers",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    domain = table.Column<string>(type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    iconfilename = table.Column<string>(name: "icon_file_name", type: "character varying", nullable: true),
                    iconcontenttype = table.Column<string>(name: "icon_content_type", type: "character varying", nullable: true),
                    iconfilesize = table.Column<long>(name: "icon_file_size", type: "bigint", nullable: true),
                    iconupdatedat = table.Column<DateTime>(name: "icon_updated_at", type: "timestamp without time zone", nullable: true),
                    trendable = table.Column<bool>(type: "boolean", nullable: true),
                    reviewedat = table.Column<DateTime>(name: "reviewed_at", type: "timestamp without time zone", nullable: true),
                    requestedreviewat = table.Column<DateTime>(name: "requested_review_at", type: "timestamp without time zone", nullable: true),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp(6) without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp(6) without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("preview_card_providers_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "preview_cards",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    url = table.Column<string>(type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    title = table.Column<string>(type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    description = table.Column<string>(type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    imagefilename = table.Column<string>(name: "image_file_name", type: "character varying", nullable: true),
                    imagecontenttype = table.Column<string>(name: "image_content_type", type: "character varying", nullable: true),
                    imagefilesize = table.Column<int>(name: "image_file_size", type: "integer", nullable: true),
                    imageupdatedat = table.Column<DateTime>(name: "image_updated_at", type: "timestamp without time zone", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: false),
                    html = table.Column<string>(type: "text", nullable: false, defaultValueSql: "''::text"),
                    authorname = table.Column<string>(name: "author_name", type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    authorurl = table.Column<string>(name: "author_url", type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    providername = table.Column<string>(name: "provider_name", type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    providerurl = table.Column<string>(name: "provider_url", type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    width = table.Column<int>(type: "integer", nullable: false),
                    height = table.Column<int>(type: "integer", nullable: false),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false),
                    embedurl = table.Column<string>(name: "embed_url", type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    imagestorageschemaversion = table.Column<int>(name: "image_storage_schema_version", type: "integer", nullable: true),
                    blurhash = table.Column<string>(type: "character varying", nullable: true),
                    language = table.Column<string>(type: "character varying", nullable: true),
                    maxscore = table.Column<double>(name: "max_score", type: "double precision", nullable: true),
                    maxscoreat = table.Column<DateTime>(name: "max_score_at", type: "timestamp without time zone", nullable: true),
                    trendable = table.Column<bool>(type: "boolean", nullable: true),
                    linktype = table.Column<int>(name: "link_type", type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("preview_cards_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "preview_cards_statuses",
                columns: table => new
                {
                    previewcardid = table.Column<long>(name: "preview_card_id", type: "bigint", nullable: false),
                    statusid = table.Column<long>(name: "status_id", type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "relays",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    inboxurl = table.Column<string>(name: "inbox_url", type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    followactivityid = table.Column<string>(name: "follow_activity_id", type: "character varying", nullable: true),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false),
                    state = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("relays_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "rules",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    deletedat = table.Column<DateTime>(name: "deleted_at", type: "timestamp without time zone", nullable: true),
                    text = table.Column<string>(type: "text", nullable: false, defaultValueSql: "''::text"),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("rules_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "settings",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    var = table.Column<string>(type: "character varying", nullable: false),
                    value = table.Column<string>(type: "text", nullable: true),
                    thingtype = table.Column<string>(name: "thing_type", type: "character varying", nullable: true),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: true),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: true),
                    thingid = table.Column<long>(name: "thing_id", type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("settings_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "site_uploads",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    var = table.Column<string>(type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    filefilename = table.Column<string>(name: "file_file_name", type: "character varying", nullable: true),
                    filecontenttype = table.Column<string>(name: "file_content_type", type: "character varying", nullable: true),
                    filefilesize = table.Column<int>(name: "file_file_size", type: "integer", nullable: true),
                    fileupdatedat = table.Column<DateTime>(name: "file_updated_at", type: "timestamp without time zone", nullable: true),
                    meta = table.Column<string>(type: "json", nullable: true),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false),
                    blurhash = table.Column<string>(type: "character varying", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("site_uploads_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "system_keys",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    key = table.Column<byte[]>(type: "bytea", nullable: true),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("system_keys_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tags",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false),
                    usable = table.Column<bool>(type: "boolean", nullable: true),
                    trendable = table.Column<bool>(type: "boolean", nullable: true),
                    listable = table.Column<bool>(type: "boolean", nullable: true),
                    reviewedat = table.Column<DateTime>(name: "reviewed_at", type: "timestamp without time zone", nullable: true),
                    requestedreviewat = table.Column<DateTime>(name: "requested_review_at", type: "timestamp without time zone", nullable: true),
                    laststatusat = table.Column<DateTime>(name: "last_status_at", type: "timestamp without time zone", nullable: true),
                    maxscore = table.Column<double>(name: "max_score", type: "double precision", nullable: true),
                    maxscoreat = table.Column<DateTime>(name: "max_score_at", type: "timestamp without time zone", nullable: true),
                    displayname = table.Column<string>(name: "display_name", type: "character varying", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tags_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "unavailable_domains",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    domain = table.Column<string>(type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("unavailable_domains_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    color = table.Column<string>(type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    position = table.Column<int>(type: "integer", nullable: false),
                    permissions = table.Column<long>(type: "bigint", nullable: false),
                    highlighted = table.Column<bool>(type: "boolean", nullable: false),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp(6) without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp(6) without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_roles_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "webhooks",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    url = table.Column<string>(type: "character varying", nullable: false),
                    events = table.Column<string[]>(type: "character varying[]", nullable: false, defaultValueSql: "'{}'::character varying[]"),
                    secret = table.Column<string>(type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    enabled = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true"),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp(6) without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp(6) without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("webhooks_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "account_aliases",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: true),
                    acct = table.Column<string>(type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    uri = table.Column<string>(type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("account_aliases_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_fc91575d08",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "account_deletion_requests",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: true),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("account_deletion_requests_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_45bf2626b9",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "account_domain_blocks",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    domain = table.Column<string>(type: "character varying", nullable: true),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("account_domain_blocks_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_206c6029bd",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "account_migrations",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: true),
                    acct = table.Column<string>(type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    followerscount = table.Column<long>(name: "followers_count", type: "bigint", nullable: false),
                    targetaccountid = table.Column<long>(name: "target_account_id", type: "bigint", nullable: true),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("account_migrations_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_c9f701caaf",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_rails_d9a8dad070",
                        column: x => x.targetaccountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "account_moderation_notes",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    content = table.Column<string>(type: "text", nullable: false),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: false),
                    targetaccountid = table.Column<long>(name: "target_account_id", type: "bigint", nullable: false),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("account_moderation_notes_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_3f8b75089b",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_rails_dd62ed5ac3",
                        column: x => x.targetaccountid,
                        principalTable: "accounts",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "account_notes",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: true),
                    targetaccountid = table.Column<long>(name: "target_account_id", type: "bigint", nullable: true),
                    comment = table.Column<string>(type: "text", nullable: false),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("account_notes_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_2801b48f1a",
                        column: x => x.targetaccountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_rails_4ee4503c69",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "account_pins",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: true),
                    targetaccountid = table.Column<long>(name: "target_account_id", type: "bigint", nullable: true),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("account_pins_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_a176e26c37",
                        column: x => x.targetaccountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_rails_d44979e5dd",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "account_stats",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: false),
                    statusescount = table.Column<long>(name: "statuses_count", type: "bigint", nullable: false),
                    followingcount = table.Column<long>(name: "following_count", type: "bigint", nullable: false),
                    followerscount = table.Column<long>(name: "followers_count", type: "bigint", nullable: false),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false),
                    laststatusat = table.Column<DateTime>(name: "last_status_at", type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("account_stats_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_215bb31ff1",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "account_statuses_cleanup_policies",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: false),
                    enabled = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true"),
                    minstatusage = table.Column<int>(name: "min_status_age", type: "integer", nullable: false, defaultValueSql: "1209600"),
                    keepdirect = table.Column<bool>(name: "keep_direct", type: "boolean", nullable: false, defaultValueSql: "true"),
                    keeppinned = table.Column<bool>(name: "keep_pinned", type: "boolean", nullable: false, defaultValueSql: "true"),
                    keeppolls = table.Column<bool>(name: "keep_polls", type: "boolean", nullable: false),
                    keepmedia = table.Column<bool>(name: "keep_media", type: "boolean", nullable: false),
                    keepselffav = table.Column<bool>(name: "keep_self_fav", type: "boolean", nullable: false, defaultValueSql: "true"),
                    keepselfbookmark = table.Column<bool>(name: "keep_self_bookmark", type: "boolean", nullable: false, defaultValueSql: "true"),
                    minfavs = table.Column<int>(name: "min_favs", type: "integer", nullable: true),
                    minreblogs = table.Column<int>(name: "min_reblogs", type: "integer", nullable: true),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp(6) without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp(6) without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("account_statuses_cleanup_policies_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_23d5f73cfe",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "admin_action_logs",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: true),
                    action = table.Column<string>(type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    targettype = table.Column<string>(name: "target_type", type: "character varying", nullable: true),
                    targetid = table.Column<long>(name: "target_id", type: "bigint", nullable: true),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false),
                    humanidentifier = table.Column<string>(name: "human_identifier", type: "character varying", nullable: true),
                    routeparam = table.Column<string>(name: "route_param", type: "character varying", nullable: true),
                    permalink = table.Column<string>(type: "character varying", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("admin_action_logs_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_a7667297fa",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "blocks",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: false),
                    targetaccountid = table.Column<long>(name: "target_account_id", type: "bigint", nullable: false),
                    uri = table.Column<string>(type: "character varying", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("blocks_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_4269e03e65",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_9571bfabc1",
                        column: x => x.targetaccountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "canonical_email_blocks",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    canonicalemailhash = table.Column<string>(name: "canonical_email_hash", type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    referenceaccountid = table.Column<long>(name: "reference_account_id", type: "bigint", nullable: true),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp(6) without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp(6) without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("canonical_email_blocks_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_1ecb262096",
                        column: x => x.referenceaccountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "custom_filters",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: true),
                    expiresat = table.Column<DateTime>(name: "expires_at", type: "timestamp without time zone", nullable: true),
                    phrase = table.Column<string>(type: "text", nullable: false, defaultValueSql: "''::text"),
                    context = table.Column<string[]>(type: "character varying[]", nullable: false, defaultValueSql: "'{}'::character varying[]"),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false),
                    action = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("custom_filters_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_8b8d786993",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "follow_recommendation_suppressions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: false),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp(6) without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp(6) without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("follow_recommendation_suppressions_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_dfb9a1dbe2",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "follow_requests",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: false),
                    targetaccountid = table.Column<long>(name: "target_account_id", type: "bigint", nullable: false),
                    showreblogs = table.Column<bool>(name: "show_reblogs", type: "boolean", nullable: false, defaultValueSql: "true"),
                    uri = table.Column<string>(type: "character varying", nullable: true),
                    notify = table.Column<bool>(type: "boolean", nullable: false),
                    languages = table.Column<string[]>(type: "character varying[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("follow_requests_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_76d644b0e7",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_9291ec025d",
                        column: x => x.targetaccountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "follows",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: false),
                    targetaccountid = table.Column<long>(name: "target_account_id", type: "bigint", nullable: false),
                    showreblogs = table.Column<bool>(name: "show_reblogs", type: "boolean", nullable: false, defaultValueSql: "true"),
                    uri = table.Column<string>(type: "character varying", nullable: true),
                    notify = table.Column<bool>(type: "boolean", nullable: false),
                    languages = table.Column<string[]>(type: "character varying[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("follows_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_32ed1b5560",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_745ca29eac",
                        column: x => x.targetaccountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "imports",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type = table.Column<int>(type: "integer", nullable: false),
                    approved = table.Column<bool>(type: "boolean", nullable: false),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false),
                    datafilename = table.Column<string>(name: "data_file_name", type: "character varying", nullable: true),
                    datacontenttype = table.Column<string>(name: "data_content_type", type: "character varying", nullable: true),
                    datafilesize = table.Column<int>(name: "data_file_size", type: "integer", nullable: true),
                    dataupdatedat = table.Column<DateTime>(name: "data_updated_at", type: "timestamp without time zone", nullable: true),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: false),
                    overwrite = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("imports_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_6db1b6e408",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "lists",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: false),
                    title = table.Column<string>(type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false),
                    repliespolicy = table.Column<int>(name: "replies_policy", type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("lists_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_3853b78dac",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mutes",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false),
                    hidenotifications = table.Column<bool>(name: "hide_notifications", type: "boolean", nullable: false, defaultValueSql: "true"),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: false),
                    targetaccountid = table.Column<long>(name: "target_account_id", type: "bigint", nullable: false),
                    expiresat = table.Column<DateTime>(name: "expires_at", type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("mutes_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_b8d8daf315",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_eecff219ea",
                        column: x => x.targetaccountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    activityid = table.Column<long>(name: "activity_id", type: "bigint", nullable: false),
                    activitytype = table.Column<string>(name: "activity_type", type: "character varying", nullable: false),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: false),
                    fromaccountid = table.Column<long>(name: "from_account_id", type: "bigint", nullable: false),
                    type = table.Column<string>(type: "character varying", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("notifications_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_c141c8ee55",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_fbd6b0bf9e",
                        column: x => x.fromaccountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reports",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    statusids = table.Column<long[]>(name: "status_ids", type: "bigint[]", nullable: false, defaultValueSql: "'{}'::bigint[]"),
                    comment = table.Column<string>(type: "text", nullable: false, defaultValueSql: "''::text"),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: false),
                    actiontakenbyaccountid = table.Column<long>(name: "action_taken_by_account_id", type: "bigint", nullable: true),
                    targetaccountid = table.Column<long>(name: "target_account_id", type: "bigint", nullable: false),
                    assignedaccountid = table.Column<long>(name: "assigned_account_id", type: "bigint", nullable: true),
                    uri = table.Column<string>(type: "character varying", nullable: true),
                    forwarded = table.Column<bool>(type: "boolean", nullable: true),
                    category = table.Column<int>(type: "integer", nullable: false),
                    actiontakenat = table.Column<DateTime>(name: "action_taken_at", type: "timestamp without time zone", nullable: true),
                    ruleids = table.Column<long[]>(name: "rule_ids", type: "bigint[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("reports_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_4b81f7522c",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_bca45b75fd",
                        column: x => x.actiontakenbyaccountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_eb37af34f0",
                        column: x => x.targetaccountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_rails_4e7a498fb4",
                        column: x => x.assignedaccountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "scheduled_statuses",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: true),
                    scheduledat = table.Column<DateTime>(name: "scheduled_at", type: "timestamp without time zone", nullable: true),
                    @params = table.Column<string>(name: "params", type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("scheduled_statuses_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_23bd9018f9",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "statuses",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "timestamp_id('statuses'::text)"),
                    uri = table.Column<string>(type: "character varying", nullable: true),
                    text = table.Column<string>(type: "text", nullable: false, defaultValueSql: "''::text"),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false),
                    inreplytoid = table.Column<long>(name: "in_reply_to_id", type: "bigint", nullable: true),
                    reblogofid = table.Column<long>(name: "reblog_of_id", type: "bigint", nullable: true),
                    url = table.Column<string>(type: "character varying", nullable: true),
                    sensitive = table.Column<bool>(type: "boolean", nullable: false),
                    visibility = table.Column<int>(type: "integer", nullable: false),
                    spoilertext = table.Column<string>(name: "spoiler_text", type: "text", nullable: false, defaultValueSql: "''::text"),
                    reply = table.Column<bool>(type: "boolean", nullable: false),
                    language = table.Column<string>(type: "character varying", nullable: true),
                    conversationid = table.Column<long>(name: "conversation_id", type: "bigint", nullable: true),
                    local = table.Column<bool>(type: "boolean", nullable: true),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: false),
                    applicationid = table.Column<long>(name: "application_id", type: "bigint", nullable: true),
                    inreplytoaccountid = table.Column<long>(name: "in_reply_to_account_id", type: "bigint", nullable: true),
                    pollid = table.Column<long>(name: "poll_id", type: "bigint", nullable: true),
                    deletedat = table.Column<DateTime>(name: "deleted_at", type: "timestamp without time zone", nullable: true),
                    editedat = table.Column<DateTime>(name: "edited_at", type: "timestamp without time zone", nullable: true),
                    trendable = table.Column<bool>(type: "boolean", nullable: true),
                    orderedmediaattachmentids = table.Column<long[]>(name: "ordered_media_attachment_ids", type: "bigint[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("statuses_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_9bda1543f7",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_c7fa917661",
                        column: x => x.inreplytoaccountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_rails_256483a9ab",
                        column: x => x.reblogofid,
                        principalTable: "statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_rails_94a6f70399",
                        column: x => x.inreplytoid,
                        principalTable: "statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "tombstones",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: true),
                    uri = table.Column<string>(type: "character varying", nullable: false),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false),
                    bymoderator = table.Column<bool>(name: "by_moderator", type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tombstones_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_f95b861449",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "announcement_mutes",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: true),
                    announcementid = table.Column<long>(name: "announcement_id", type: "bigint", nullable: true),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("announcement_mutes_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_9c99f8e835",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_rails_e35401adf1",
                        column: x => x.announcementid,
                        principalTable: "announcements",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "account_conversations",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: true),
                    conversationid = table.Column<long>(name: "conversation_id", type: "bigint", nullable: true),
                    participantaccountids = table.Column<long[]>(name: "participant_account_ids", type: "bigint[]", nullable: false, defaultValueSql: "'{}'::bigint[]"),
                    statusids = table.Column<long[]>(name: "status_ids", type: "bigint[]", nullable: false, defaultValueSql: "'{}'::bigint[]"),
                    laststatusid = table.Column<long>(name: "last_status_id", type: "bigint", nullable: true),
                    lockversion = table.Column<int>(name: "lock_version", type: "integer", nullable: false),
                    unread = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("account_conversations_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_1491654f9f",
                        column: x => x.conversationid,
                        principalTable: "conversations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_rails_6f5278b6e9",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "conversation_mutes",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    conversationid = table.Column<long>(name: "conversation_id", type: "bigint", nullable: false),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("conversation_mutes_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_225b4212bb",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_rails_5ab139311f",
                        column: x => x.conversationid,
                        principalTable: "conversations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "announcement_reactions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: true),
                    announcementid = table.Column<long>(name: "announcement_id", type: "bigint", nullable: true),
                    name = table.Column<string>(type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    customemojiid = table.Column<long>(name: "custom_emoji_id", type: "bigint", nullable: true),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("announcement_reactions_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_7444ad831f",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_rails_a1226eaa5c",
                        column: x => x.announcementid,
                        principalTable: "announcements",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_rails_b742c91c0e",
                        column: x => x.customemojiid,
                        principalTable: "custom_emojis",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "preview_card_trends",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    previewcardid = table.Column<long>(name: "preview_card_id", type: "bigint", nullable: false),
                    score = table.Column<double>(type: "double precision", nullable: false),
                    rank = table.Column<int>(type: "integer", nullable: false),
                    allowed = table.Column<bool>(type: "boolean", nullable: false),
                    language = table.Column<string>(type: "character varying", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("preview_card_trends_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_371593db34",
                        column: x => x.previewcardid,
                        principalTable: "preview_cards",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "featured_tags",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: false),
                    tagid = table.Column<long>(name: "tag_id", type: "bigint", nullable: false),
                    statusescount = table.Column<long>(name: "statuses_count", type: "bigint", nullable: false),
                    laststatusat = table.Column<DateTime>(name: "last_status_at", type: "timestamp without time zone", nullable: true),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false),
                    name = table.Column<string>(type: "character varying", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("featured_tags_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_174efcf15f",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_rails_23a9055c7c",
                        column: x => x.tagid,
                        principalTable: "tags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tag_follows",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tagid = table.Column<long>(name: "tag_id", type: "bigint", nullable: false),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: false),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp(6) without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp(6) without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tag_follows_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_091e831473",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_rails_0deefe597f",
                        column: x => x.tagid,
                        principalTable: "tags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "custom_filter_keywords",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    customfilterid = table.Column<long>(name: "custom_filter_id", type: "bigint", nullable: false),
                    keyword = table.Column<string>(type: "text", nullable: false, defaultValueSql: "''::text"),
                    wholeword = table.Column<bool>(name: "whole_word", type: "boolean", nullable: false, defaultValueSql: "true"),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp(6) without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp(6) without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("custom_filter_keywords_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_5a49a74012",
                        column: x => x.customfilterid,
                        principalTable: "custom_filters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "list_accounts",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    listid = table.Column<long>(name: "list_id", type: "bigint", nullable: false),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: false),
                    followid = table.Column<long>(name: "follow_id", type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("list_accounts_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_40f9cc29f1",
                        column: x => x.followid,
                        principalTable: "follows",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_rails_85fee9d6ab",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_rails_e54e356c88",
                        column: x => x.listid,
                        principalTable: "lists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "account_warnings",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: true),
                    targetaccountid = table.Column<long>(name: "target_account_id", type: "bigint", nullable: true),
                    action = table.Column<int>(type: "integer", nullable: false),
                    text = table.Column<string>(type: "text", nullable: false, defaultValueSql: "''::text"),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false),
                    reportid = table.Column<long>(name: "report_id", type: "bigint", nullable: true),
                    statusids = table.Column<string[]>(name: "status_ids", type: "character varying[]", nullable: true),
                    overruledat = table.Column<DateTime>(name: "overruled_at", type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("account_warnings_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_8f2bab4b16",
                        column: x => x.reportid,
                        principalTable: "reports",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_rails_a65a1bf71b",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_rails_a7ebbb1e37",
                        column: x => x.targetaccountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "report_notes",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    content = table.Column<string>(type: "text", nullable: false),
                    reportid = table.Column<long>(name: "report_id", type: "bigint", nullable: false),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: false),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("report_notes_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_7fa83a61eb",
                        column: x => x.reportid,
                        principalTable: "reports",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_rails_cae66353f3",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "bookmarks",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: false),
                    statusid = table.Column<long>(name: "status_id", type: "bigint", nullable: false),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("bookmarks_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_11207ffcfd",
                        column: x => x.statusid,
                        principalTable: "statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_rails_9f6ac182a6",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "custom_filter_statuses",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    customfilterid = table.Column<long>(name: "custom_filter_id", type: "bigint", nullable: false),
                    statusid = table.Column<long>(name: "status_id", type: "bigint", nullable: false),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp(6) without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp(6) without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("custom_filter_statuses_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_2f6d20c0cf",
                        column: x => x.statusid,
                        principalTable: "statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_rails_e2ddaf5b14",
                        column: x => x.customfilterid,
                        principalTable: "custom_filters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "favourites",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: false),
                    statusid = table.Column<long>(name: "status_id", type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("favourites_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_5eb6c2b873",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_b0e856845e",
                        column: x => x.statusid,
                        principalTable: "statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "media_attachments",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "timestamp_id('media_attachments'::text)"),
                    statusid = table.Column<long>(name: "status_id", type: "bigint", nullable: true),
                    filefilename = table.Column<string>(name: "file_file_name", type: "character varying", nullable: true),
                    filecontenttype = table.Column<string>(name: "file_content_type", type: "character varying", nullable: true),
                    filefilesize = table.Column<int>(name: "file_file_size", type: "integer", nullable: true),
                    fileupdatedat = table.Column<DateTime>(name: "file_updated_at", type: "timestamp without time zone", nullable: true),
                    remoteurl = table.Column<string>(name: "remote_url", type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false),
                    shortcode = table.Column<string>(type: "character varying", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: false),
                    filemeta = table.Column<string>(name: "file_meta", type: "json", nullable: true),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    scheduledstatusid = table.Column<long>(name: "scheduled_status_id", type: "bigint", nullable: true),
                    blurhash = table.Column<string>(type: "character varying", nullable: true),
                    processing = table.Column<int>(type: "integer", nullable: true),
                    filestorageschemaversion = table.Column<int>(name: "file_storage_schema_version", type: "integer", nullable: true),
                    thumbnailfilename = table.Column<string>(name: "thumbnail_file_name", type: "character varying", nullable: true),
                    thumbnailcontenttype = table.Column<string>(name: "thumbnail_content_type", type: "character varying", nullable: true),
                    thumbnailfilesize = table.Column<int>(name: "thumbnail_file_size", type: "integer", nullable: true),
                    thumbnailupdatedat = table.Column<DateTime>(name: "thumbnail_updated_at", type: "timestamp without time zone", nullable: true),
                    thumbnailremoteurl = table.Column<string>(name: "thumbnail_remote_url", type: "character varying", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("media_attachments_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_96dd81e81b",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_rails_31fc5aeef1",
                        column: x => x.scheduledstatusid,
                        principalTable: "scheduled_statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_rails_3ec0cfdd70",
                        column: x => x.statusid,
                        principalTable: "statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "mentions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    statusid = table.Column<long>(name: "status_id", type: "bigint", nullable: true),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: true),
                    silent = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("mentions_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_970d43f9d1",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_rails_59edbe2887",
                        column: x => x.statusid,
                        principalTable: "statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "polls",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: true),
                    statusid = table.Column<long>(name: "status_id", type: "bigint", nullable: true),
                    expiresat = table.Column<DateTime>(name: "expires_at", type: "timestamp without time zone", nullable: true),
                    options = table.Column<string[]>(type: "character varying[]", nullable: false, defaultValueSql: "'{}'::character varying[]"),
                    cachedtallies = table.Column<long[]>(name: "cached_tallies", type: "bigint[]", nullable: false, defaultValueSql: "'{}'::bigint[]"),
                    multiple = table.Column<bool>(type: "boolean", nullable: false),
                    hidetotals = table.Column<bool>(name: "hide_totals", type: "boolean", nullable: false),
                    votescount = table.Column<long>(name: "votes_count", type: "bigint", nullable: false),
                    lastfetchedat = table.Column<DateTime>(name: "last_fetched_at", type: "timestamp without time zone", nullable: true),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false),
                    lockversion = table.Column<int>(name: "lock_version", type: "integer", nullable: false),
                    voterscount = table.Column<long>(name: "voters_count", type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("polls_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_3e0d9f1115",
                        column: x => x.statusid,
                        principalTable: "statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_rails_5b19a0c011",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "status_edits",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    statusid = table.Column<long>(name: "status_id", type: "bigint", nullable: false),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: true),
                    text = table.Column<string>(type: "text", nullable: false, defaultValueSql: "''::text"),
                    spoilertext = table.Column<string>(name: "spoiler_text", type: "text", nullable: false, defaultValueSql: "''::text"),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp(6) without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp(6) without time zone", nullable: false),
                    orderedmediaattachmentids = table.Column<long[]>(name: "ordered_media_attachment_ids", type: "bigint[]", nullable: true),
                    mediadescriptions = table.Column<string[]>(name: "media_descriptions", type: "text[]", nullable: true),
                    polloptions = table.Column<string[]>(name: "poll_options", type: "character varying[]", nullable: true),
                    sensitive = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("status_edits_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_a960f234a0",
                        column: x => x.statusid,
                        principalTable: "statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_rails_dc8988c545",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "status_pins",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: false),
                    statusid = table.Column<long>(name: "status_id", type: "bigint", nullable: false),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("status_pins_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_d4cb435b62",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_rails_65c05552f1",
                        column: x => x.statusid,
                        principalTable: "statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "status_stats",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    statusid = table.Column<long>(name: "status_id", type: "bigint", nullable: false),
                    repliescount = table.Column<long>(name: "replies_count", type: "bigint", nullable: false),
                    reblogscount = table.Column<long>(name: "reblogs_count", type: "bigint", nullable: false),
                    favouritescount = table.Column<long>(name: "favourites_count", type: "bigint", nullable: false),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("status_stats_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_4a247aac42",
                        column: x => x.statusid,
                        principalTable: "statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "status_trends",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    statusid = table.Column<long>(name: "status_id", type: "bigint", nullable: false),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: false),
                    score = table.Column<double>(type: "double precision", nullable: false),
                    rank = table.Column<int>(type: "integer", nullable: false),
                    allowed = table.Column<bool>(type: "boolean", nullable: false),
                    language = table.Column<string>(type: "character varying", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("status_trends_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_68c610dc1a",
                        column: x => x.statusid,
                        principalTable: "statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_rails_a6b527ea49",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "statuses_tags",
                columns: table => new
                {
                    statusid = table.Column<long>(name: "status_id", type: "bigint", nullable: false),
                    tagid = table.Column<long>(name: "tag_id", type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "fk_3081861e21",
                        column: x => x.tagid,
                        principalTable: "tags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_rails_df0fe11427",
                        column: x => x.statusid,
                        principalTable: "statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "appeals",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: false),
                    accountwarningid = table.Column<long>(name: "account_warning_id", type: "bigint", nullable: false),
                    text = table.Column<string>(type: "text", nullable: false, defaultValueSql: "''::text"),
                    approvedat = table.Column<DateTime>(name: "approved_at", type: "timestamp without time zone", nullable: true),
                    approvedbyaccountid = table.Column<long>(name: "approved_by_account_id", type: "bigint", nullable: true),
                    rejectedat = table.Column<DateTime>(name: "rejected_at", type: "timestamp without time zone", nullable: true),
                    rejectedbyaccountid = table.Column<long>(name: "rejected_by_account_id", type: "bigint", nullable: true),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp(6) without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp(6) without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("appeals_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_501c3a6e13",
                        column: x => x.rejectedbyaccountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_rails_9deb2f63ad",
                        column: x => x.approvedbyaccountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_rails_a99f14546e",
                        column: x => x.accountwarningid,
                        principalTable: "account_warnings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_rails_ea84881569",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "poll_votes",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: true),
                    pollid = table.Column<long>(name: "poll_id", type: "bigint", nullable: true),
                    choice = table.Column<int>(type: "integer", nullable: false),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false),
                    uri = table.Column<string>(type: "character varying", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("poll_votes_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_a6e6974b7e",
                        column: x => x.pollid,
                        principalTable: "polls",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_rails_b6c18cf44a",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "backups",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userid = table.Column<long>(name: "user_id", type: "bigint", nullable: true),
                    dumpfilename = table.Column<string>(name: "dump_file_name", type: "character varying", nullable: true),
                    dumpcontenttype = table.Column<string>(name: "dump_content_type", type: "character varying", nullable: true),
                    dumpupdatedat = table.Column<DateTime>(name: "dump_updated_at", type: "timestamp without time zone", nullable: true),
                    processed = table.Column<bool>(type: "boolean", nullable: false),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false),
                    dumpfilesize = table.Column<long>(name: "dump_file_size", type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("backups_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "devices",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    accesstokenid = table.Column<long>(name: "access_token_id", type: "bigint", nullable: true),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: true),
                    deviceid = table.Column<string>(name: "device_id", type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    name = table.Column<string>(type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    fingerprintkey = table.Column<string>(name: "fingerprint_key", type: "text", nullable: false, defaultValueSql: "''::text"),
                    identitykey = table.Column<string>(name: "identity_key", type: "text", nullable: false),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("devices_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_a796b75798",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "encrypted_messages",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "timestamp_id('encrypted_messages'::text)"),
                    deviceid = table.Column<long>(name: "device_id", type: "bigint", nullable: true),
                    fromaccountid = table.Column<long>(name: "from_account_id", type: "bigint", nullable: true),
                    fromdeviceid = table.Column<string>(name: "from_device_id", type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    type = table.Column<int>(type: "integer", nullable: false),
                    body = table.Column<string>(type: "text", nullable: false, defaultValueSql: "''::text"),
                    digest = table.Column<string>(type: "text", nullable: false, defaultValueSql: "''::text"),
                    messagefranking = table.Column<string>(name: "message_franking", type: "text", nullable: false, defaultValueSql: "''::text"),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("encrypted_messages_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_a42ad0f8d5",
                        column: x => x.fromaccountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_rails_a83e4df7ae",
                        column: x => x.deviceid,
                        principalTable: "devices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "one_time_keys",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    deviceid = table.Column<long>(name: "device_id", type: "bigint", nullable: true),
                    keyid = table.Column<string>(name: "key_id", type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    key = table.Column<string>(type: "text", nullable: false, defaultValueSql: "''::text"),
                    signature = table.Column<string>(type: "text", nullable: false, defaultValueSql: "''::text"),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("one_time_keys_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_d3edd8c878",
                        column: x => x.deviceid,
                        principalTable: "devices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "identities",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    provider = table.Column<string>(type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    uid = table.Column<string>(type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false),
                    userid = table.Column<long>(name: "user_id", type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("identities_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "invites",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userid = table.Column<long>(name: "user_id", type: "bigint", nullable: false),
                    code = table.Column<string>(type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    expiresat = table.Column<DateTime>(name: "expires_at", type: "timestamp without time zone", nullable: true),
                    maxuses = table.Column<int>(name: "max_uses", type: "integer", nullable: true),
                    uses = table.Column<int>(type: "integer", nullable: false),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false),
                    autofollow = table.Column<bool>(type: "boolean", nullable: false),
                    comment = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("invites_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "login_activities",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userid = table.Column<long>(name: "user_id", type: "bigint", nullable: false),
                    authenticationmethod = table.Column<string>(name: "authentication_method", type: "character varying", nullable: true),
                    provider = table.Column<string>(type: "character varying", nullable: true),
                    success = table.Column<bool>(type: "boolean", nullable: true),
                    failurereason = table.Column<string>(name: "failure_reason", type: "character varying", nullable: true),
                    ip = table.Column<IPAddress>(type: "inet", nullable: true),
                    useragent = table.Column<string>(name: "user_agent", type: "character varying", nullable: true),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("login_activities_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "markers",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userid = table.Column<long>(name: "user_id", type: "bigint", nullable: true),
                    timeline = table.Column<string>(type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    lastreadid = table.Column<long>(name: "last_read_id", type: "bigint", nullable: false),
                    lockversion = table.Column<int>(name: "lock_version", type: "integer", nullable: false),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("markers_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "oauth_access_grants",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    token = table.Column<string>(type: "character varying", nullable: false),
                    expiresin = table.Column<int>(name: "expires_in", type: "integer", nullable: false),
                    redirecturi = table.Column<string>(name: "redirect_uri", type: "text", nullable: false),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    revokedat = table.Column<DateTime>(name: "revoked_at", type: "timestamp without time zone", nullable: true),
                    scopes = table.Column<string>(type: "character varying", nullable: true),
                    applicationid = table.Column<long>(name: "application_id", type: "bigint", nullable: false),
                    resourceownerid = table.Column<long>(name: "resource_owner_id", type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("oauth_access_grants_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "oauth_access_tokens",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    token = table.Column<string>(type: "character varying", nullable: false),
                    refreshtoken = table.Column<string>(name: "refresh_token", type: "character varying", nullable: true),
                    expiresin = table.Column<int>(name: "expires_in", type: "integer", nullable: true),
                    revokedat = table.Column<DateTime>(name: "revoked_at", type: "timestamp without time zone", nullable: true),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    scopes = table.Column<string>(type: "character varying", nullable: true),
                    applicationid = table.Column<long>(name: "application_id", type: "bigint", nullable: true),
                    resourceownerid = table.Column<long>(name: "resource_owner_id", type: "bigint", nullable: true),
                    lastusedat = table.Column<DateTime>(name: "last_used_at", type: "timestamp without time zone", nullable: true),
                    lastusedip = table.Column<IPAddress>(name: "last_used_ip", type: "inet", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("oauth_access_tokens_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "oauth_applications",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying", nullable: false),
                    uid = table.Column<string>(type: "character varying", nullable: false),
                    secret = table.Column<string>(type: "character varying", nullable: false),
                    redirecturi = table.Column<string>(name: "redirect_uri", type: "text", nullable: false),
                    scopes = table.Column<string>(type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: true),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: true),
                    superapp = table.Column<bool>(type: "boolean", nullable: false),
                    website = table.Column<string>(type: "character varying", nullable: true),
                    ownertype = table.Column<string>(name: "owner_type", type: "character varying", nullable: true),
                    ownerid = table.Column<long>(name: "owner_id", type: "bigint", nullable: true),
                    confidential = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true")
                },
                constraints: table =>
                {
                    table.PrimaryKey("oauth_applications_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    email = table.Column<string>(type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false),
                    encryptedpassword = table.Column<string>(name: "encrypted_password", type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    resetpasswordtoken = table.Column<string>(name: "reset_password_token", type: "character varying", nullable: true),
                    resetpasswordsentat = table.Column<DateTime>(name: "reset_password_sent_at", type: "timestamp without time zone", nullable: true),
                    signincount = table.Column<int>(name: "sign_in_count", type: "integer", nullable: false),
                    currentsigninat = table.Column<DateTime>(name: "current_sign_in_at", type: "timestamp without time zone", nullable: true),
                    lastsigninat = table.Column<DateTime>(name: "last_sign_in_at", type: "timestamp without time zone", nullable: true),
                    admin = table.Column<bool>(type: "boolean", nullable: false),
                    confirmationtoken = table.Column<string>(name: "confirmation_token", type: "character varying", nullable: true),
                    confirmedat = table.Column<DateTime>(name: "confirmed_at", type: "timestamp without time zone", nullable: true),
                    confirmationsentat = table.Column<DateTime>(name: "confirmation_sent_at", type: "timestamp without time zone", nullable: true),
                    unconfirmedemail = table.Column<string>(name: "unconfirmed_email", type: "character varying", nullable: true),
                    locale = table.Column<string>(type: "character varying", nullable: true),
                    encryptedotpsecret = table.Column<string>(name: "encrypted_otp_secret", type: "character varying", nullable: true),
                    encryptedotpsecretiv = table.Column<string>(name: "encrypted_otp_secret_iv", type: "character varying", nullable: true),
                    encryptedotpsecretsalt = table.Column<string>(name: "encrypted_otp_secret_salt", type: "character varying", nullable: true),
                    consumedtimestep = table.Column<int>(name: "consumed_timestep", type: "integer", nullable: true),
                    otprequiredforlogin = table.Column<bool>(name: "otp_required_for_login", type: "boolean", nullable: false),
                    lastemailedat = table.Column<DateTime>(name: "last_emailed_at", type: "timestamp without time zone", nullable: true),
                    otpbackupcodes = table.Column<string[]>(name: "otp_backup_codes", type: "character varying[]", nullable: true),
                    accountid = table.Column<long>(name: "account_id", type: "bigint", nullable: false),
                    disabled = table.Column<bool>(type: "boolean", nullable: false),
                    moderator = table.Column<bool>(type: "boolean", nullable: false),
                    inviteid = table.Column<long>(name: "invite_id", type: "bigint", nullable: true),
                    chosenlanguages = table.Column<string[]>(name: "chosen_languages", type: "character varying[]", nullable: true),
                    createdbyapplicationid = table.Column<long>(name: "created_by_application_id", type: "bigint", nullable: true),
                    approved = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true"),
                    signintoken = table.Column<string>(name: "sign_in_token", type: "character varying", nullable: true),
                    signintokensentat = table.Column<DateTime>(name: "sign_in_token_sent_at", type: "timestamp without time zone", nullable: true),
                    webauthnid = table.Column<string>(name: "webauthn_id", type: "character varying", nullable: true),
                    signupip = table.Column<IPAddress>(name: "sign_up_ip", type: "inet", nullable: true),
                    skipsignintoken = table.Column<bool>(name: "skip_sign_in_token", type: "boolean", nullable: true),
                    roleid = table.Column<long>(name: "role_id", type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("users_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_50500f500d",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_rails_642f17018b",
                        column: x => x.roleid,
                        principalTable: "user_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_rails_8fb2a43e88",
                        column: x => x.inviteid,
                        principalTable: "invites",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_rails_ecc9536e7c",
                        column: x => x.createdbyapplicationid,
                        principalTable: "oauth_applications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "session_activations",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sessionid = table.Column<string>(name: "session_id", type: "character varying", nullable: false),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false),
                    useragent = table.Column<string>(name: "user_agent", type: "character varying", nullable: false, defaultValueSql: "''::character varying"),
                    ip = table.Column<IPAddress>(type: "inet", nullable: true),
                    accesstokenid = table.Column<long>(name: "access_token_id", type: "bigint", nullable: true),
                    userid = table.Column<long>(name: "user_id", type: "bigint", nullable: false),
                    webpushsubscriptionid = table.Column<long>(name: "web_push_subscription_id", type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("session_activations_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_957e5bda89",
                        column: x => x.accesstokenid,
                        principalTable: "oauth_access_tokens",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_e5fda67334",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_invite_requests",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userid = table.Column<long>(name: "user_id", type: "bigint", nullable: true),
                    text = table.Column<string>(type: "text", nullable: true),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_invite_requests_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_3773f15361",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "web_push_subscriptions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    endpoint = table.Column<string>(type: "character varying", nullable: false),
                    keyp256dh = table.Column<string>(name: "key_p256dh", type: "character varying", nullable: false),
                    keyauth = table.Column<string>(name: "key_auth", type: "character varying", nullable: false),
                    data = table.Column<string>(type: "json", nullable: true),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false),
                    accesstokenid = table.Column<long>(name: "access_token_id", type: "bigint", nullable: true),
                    userid = table.Column<long>(name: "user_id", type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("web_push_subscriptions_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_751a9f390b",
                        column: x => x.accesstokenid,
                        principalTable: "oauth_access_tokens",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_rails_b006f28dac",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "web_settings",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    data = table.Column<string>(type: "json", nullable: true),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false),
                    userid = table.Column<long>(name: "user_id", type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("web_settings_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_11910667b2",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "webauthn_credentials",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    externalid = table.Column<string>(name: "external_id", type: "character varying", nullable: false),
                    publickey = table.Column<string>(name: "public_key", type: "character varying", nullable: false),
                    nickname = table.Column<string>(type: "character varying", nullable: false),
                    signcount = table.Column<long>(name: "sign_count", type: "bigint", nullable: false),
                    userid = table.Column<long>(name: "user_id", type: "bigint", nullable: true),
                    createdat = table.Column<DateTime>(name: "created_at", type: "timestamp without time zone", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("webauthn_credentials_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_rails_a4355aef77",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.Sql(AccountsSearchIndex);

            migrationBuilder.Sql(AccountsUsernameAndDomainLowerIndex);

            migrationBuilder.Sql(TagsNameLowerBtreeIndex);
            
            migrationBuilder.CreateIndex(
                name: "index_ip_blocks_on_ip",
                table: "ip_blocks",
                column: "ip");

            migrationBuilder.CreateIndex(
                name: "index_account_aliases_on_account_id",
                table: "account_aliases",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "index_account_conversations_on_conversation_id",
                table: "account_conversations",
                column: "conversation_id");

            migrationBuilder.CreateIndex(
                name: "index_unique_conversations",
                table: "account_conversations",
                columns: new[] { "account_id", "conversation_id", "participant_account_ids" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_account_deletion_requests_on_account_id",
                table: "account_deletion_requests",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "index_account_domain_blocks_on_account_id_and_domain",
                table: "account_domain_blocks",
                columns: new[] { "account_id", "domain" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_account_migrations_on_account_id",
                table: "account_migrations",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "index_account_migrations_on_target_account_id",
                table: "account_migrations",
                column: "target_account_id",
                filter: "(target_account_id IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "index_account_moderation_notes_on_account_id",
                table: "account_moderation_notes",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "index_account_moderation_notes_on_target_account_id",
                table: "account_moderation_notes",
                column: "target_account_id");

            migrationBuilder.CreateIndex(
                name: "index_account_notes_on_account_id_and_target_account_id",
                table: "account_notes",
                columns: new[] { "account_id", "target_account_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_account_notes_on_target_account_id",
                table: "account_notes",
                column: "target_account_id");

            migrationBuilder.CreateIndex(
                name: "index_account_pins_on_account_id_and_target_account_id",
                table: "account_pins",
                columns: new[] { "account_id", "target_account_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_account_pins_on_target_account_id",
                table: "account_pins",
                column: "target_account_id");

            migrationBuilder.CreateIndex(
                name: "index_account_stats_on_account_id",
                table: "account_stats",
                column: "account_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_account_statuses_cleanup_policies_on_account_id",
                table: "account_statuses_cleanup_policies",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "index_account_warnings_on_account_id",
                table: "account_warnings",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "index_account_warnings_on_target_account_id",
                table: "account_warnings",
                column: "target_account_id");

            migrationBuilder.CreateIndex(
                name: "index_accounts_on_moved_to_account_id",
                table: "accounts",
                column: "moved_to_account_id",
                filter: "(moved_to_account_id IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "index_accounts_on_uri",
                table: "accounts",
                column: "uri");

            migrationBuilder.CreateIndex(
                name: "index_accounts_on_url",
                table: "accounts",
                column: "url",
                filter: "(url IS NOT NULL)")
                .Annotation("Npgsql:IndexOperators", new[] { "text_pattern_ops" });

            migrationBuilder.CreateIndex(
                name: "index_accounts_tags_on_account_id_and_tag_id",
                table: "accounts_tags",
                columns: new[] { "account_id", "tag_id" });

            migrationBuilder.CreateIndex(
                name: "index_accounts_tags_on_tag_id_and_account_id",
                table: "accounts_tags",
                columns: new[] { "tag_id", "account_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_admin_action_logs_on_account_id",
                table: "admin_action_logs",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "index_admin_action_logs_on_target_type_and_target_id",
                table: "admin_action_logs",
                columns: new[] { "target_type", "target_id" });

            migrationBuilder.CreateIndex(
                name: "index_announcement_mutes_on_account_id_and_announcement_id",
                table: "announcement_mutes",
                columns: new[] { "account_id", "announcement_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_announcement_mutes_on_announcement_id",
                table: "announcement_mutes",
                column: "announcement_id");

            migrationBuilder.CreateIndex(
                name: "index_announcement_reactions_on_account_id_and_announcement_id",
                table: "announcement_reactions",
                columns: new[] { "account_id", "announcement_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_announcement_reactions_on_announcement_id",
                table: "announcement_reactions",
                column: "announcement_id");

            migrationBuilder.CreateIndex(
                name: "index_announcement_reactions_on_custom_emoji_id",
                table: "announcement_reactions",
                column: "custom_emoji_id",
                filter: "(custom_emoji_id IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "index_appeals_on_account_id",
                table: "appeals",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "index_appeals_on_account_warning_id",
                table: "appeals",
                column: "account_warning_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_appeals_on_approved_by_account_id",
                table: "appeals",
                column: "approved_by_account_id",
                filter: "(approved_by_account_id IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "index_appeals_on_rejected_by_account_id",
                table: "appeals",
                column: "rejected_by_account_id",
                filter: "(rejected_by_account_id IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "index_blocks_on_account_id_and_target_account_id",
                table: "blocks",
                columns: new[] { "account_id", "target_account_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_blocks_on_target_account_id",
                table: "blocks",
                column: "target_account_id");

            migrationBuilder.CreateIndex(
                name: "index_bookmarks_on_account_id_and_status_id",
                table: "bookmarks",
                columns: new[] { "account_id", "status_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_bookmarks_on_status_id",
                table: "bookmarks",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "index_canonical_email_blocks_on_canonical_email_hash",
                table: "canonical_email_blocks",
                column: "canonical_email_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_canonical_email_blocks_on_reference_account_id",
                table: "canonical_email_blocks",
                column: "reference_account_id");

            migrationBuilder.CreateIndex(
                name: "index_conversation_mutes_on_account_id_and_conversation_id",
                table: "conversation_mutes",
                columns: new[] { "account_id", "conversation_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_conversations_on_uri",
                table: "conversations",
                column: "uri",
                unique: true,
                filter: "(uri IS NOT NULL)")
                .Annotation("Npgsql:IndexOperators", new[] { "text_pattern_ops" });

            migrationBuilder.CreateIndex(
                name: "index_custom_emoji_categories_on_name",
                table: "custom_emoji_categories",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_custom_emojis_on_shortcode_and_domain",
                table: "custom_emojis",
                columns: new[] { "shortcode", "domain" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_custom_filter_keywords_on_custom_filter_id",
                table: "custom_filter_keywords",
                column: "custom_filter_id");

            migrationBuilder.CreateIndex(
                name: "index_custom_filter_statuses_on_custom_filter_id",
                table: "custom_filter_statuses",
                column: "custom_filter_id");

            migrationBuilder.CreateIndex(
                name: "index_custom_filter_statuses_on_status_id",
                table: "custom_filter_statuses",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "index_custom_filters_on_account_id",
                table: "custom_filters",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "index_devices_on_access_token_id",
                table: "devices",
                column: "access_token_id");

            migrationBuilder.CreateIndex(
                name: "index_devices_on_account_id",
                table: "devices",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "index_domain_allows_on_domain",
                table: "domain_allows",
                column: "domain",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_domain_blocks_on_domain",
                table: "domain_blocks",
                column: "domain",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_email_domain_blocks_on_domain",
                table: "email_domain_blocks",
                column: "domain",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_encrypted_messages_on_device_id",
                table: "encrypted_messages",
                column: "device_id");

            migrationBuilder.CreateIndex(
                name: "index_encrypted_messages_on_from_account_id",
                table: "encrypted_messages",
                column: "from_account_id");

            migrationBuilder.CreateIndex(
                name: "index_favourites_on_account_id_and_id",
                table: "favourites",
                columns: new[] { "account_id", "id" });

            migrationBuilder.CreateIndex(
                name: "index_favourites_on_account_id_and_status_id",
                table: "favourites",
                columns: new[] { "account_id", "status_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_favourites_on_status_id",
                table: "favourites",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "index_featured_tags_on_account_id_and_tag_id",
                table: "featured_tags",
                columns: new[] { "account_id", "tag_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_featured_tags_on_tag_id",
                table: "featured_tags",
                column: "tag_id");

            migrationBuilder.CreateIndex(
                name: "index_follow_recommendation_suppressions_on_account_id",
                table: "follow_recommendation_suppressions",
                column: "account_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_follow_requests_on_account_id_and_target_account_id",
                table: "follow_requests",
                columns: new[] { "account_id", "target_account_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_follows_on_account_id_and_target_account_id",
                table: "follows",
                columns: new[] { "account_id", "target_account_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_identities_on_user_id",
                table: "identities",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "index_invites_on_code",
                table: "invites",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_invites_on_user_id",
                table: "invites",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "index_list_accounts_on_account_id_and_list_id",
                table: "list_accounts",
                columns: new[] { "account_id", "list_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_list_accounts_on_follow_id",
                table: "list_accounts",
                column: "follow_id",
                filter: "(follow_id IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "index_list_accounts_on_list_id_and_account_id",
                table: "list_accounts",
                columns: new[] { "list_id", "account_id" });

            migrationBuilder.CreateIndex(
                name: "index_lists_on_account_id",
                table: "lists",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "index_login_activities_on_user_id",
                table: "login_activities",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "index_markers_on_user_id_and_timeline",
                table: "markers",
                columns: new[] { "user_id", "timeline" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_media_attachments_on_account_id_and_status_id",
                table: "media_attachments",
                columns: new[] { "account_id", "status_id" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "index_media_attachments_on_scheduled_status_id",
                table: "media_attachments",
                column: "scheduled_status_id",
                filter: "(scheduled_status_id IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "index_media_attachments_on_shortcode",
                table: "media_attachments",
                column: "shortcode",
                unique: true,
                filter: "(shortcode IS NOT NULL)")
                .Annotation("Npgsql:IndexOperators", new[] { "text_pattern_ops" });

            migrationBuilder.CreateIndex(
                name: "index_media_attachments_on_status_id",
                table: "media_attachments",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "index_mentions_on_account_id_and_status_id",
                table: "mentions",
                columns: new[] { "account_id", "status_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_mentions_on_status_id",
                table: "mentions",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "index_mutes_on_account_id_and_target_account_id",
                table: "mutes",
                columns: new[] { "account_id", "target_account_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_mutes_on_target_account_id",
                table: "mutes",
                column: "target_account_id");

            migrationBuilder.CreateIndex(
                name: "index_notifications_on_account_id_and_id_and_type",
                table: "notifications",
                columns: new[] { "account_id", "id", "type" },
                descending: new[] { false, true, false });

            migrationBuilder.CreateIndex(
                name: "index_notifications_on_activity_id_and_activity_type",
                table: "notifications",
                columns: new[] { "activity_id", "activity_type" });

            migrationBuilder.CreateIndex(
                name: "index_notifications_on_from_account_id",
                table: "notifications",
                column: "from_account_id");

            migrationBuilder.CreateIndex(
                name: "index_oauth_access_grants_on_resource_owner_id",
                table: "oauth_access_grants",
                column: "resource_owner_id");

            migrationBuilder.CreateIndex(
                name: "index_oauth_access_grants_on_token",
                table: "oauth_access_grants",
                column: "token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_oauth_access_tokens_on_refresh_token",
                table: "oauth_access_tokens",
                column: "refresh_token",
                unique: true,
                filter: "(refresh_token IS NOT NULL)")
                .Annotation("Npgsql:IndexOperators", new[] { "text_pattern_ops" });

            migrationBuilder.CreateIndex(
                name: "index_oauth_access_tokens_on_resource_owner_id",
                table: "oauth_access_tokens",
                column: "resource_owner_id",
                filter: "(resource_owner_id IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "index_oauth_access_tokens_on_token",
                table: "oauth_access_tokens",
                column: "token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_oauth_applications_on_owner_id_and_owner_type",
                table: "oauth_applications",
                columns: new[] { "owner_id", "owner_type" });

            migrationBuilder.CreateIndex(
                name: "index_oauth_applications_on_uid",
                table: "oauth_applications",
                column: "uid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_one_time_keys_on_device_id",
                table: "one_time_keys",
                column: "device_id");

            migrationBuilder.CreateIndex(
                name: "index_one_time_keys_on_key_id",
                table: "one_time_keys",
                column: "key_id");

            migrationBuilder.CreateIndex(
                name: "index_pghero_space_stats_on_database_and_captured_at",
                table: "pghero_space_stats",
                columns: new[] { "database", "captured_at" });

            migrationBuilder.CreateIndex(
                name: "index_poll_votes_on_account_id",
                table: "poll_votes",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "index_poll_votes_on_poll_id",
                table: "poll_votes",
                column: "poll_id");

            migrationBuilder.CreateIndex(
                name: "index_polls_on_account_id",
                table: "polls",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "index_polls_on_status_id",
                table: "polls",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "index_preview_card_providers_on_domain",
                table: "preview_card_providers",
                column: "domain",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_preview_card_trends_on_preview_card_id",
                table: "preview_card_trends",
                column: "preview_card_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_preview_cards_on_url",
                table: "preview_cards",
                column: "url",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_preview_cards_statuses_on_status_id_and_preview_card_id",
                table: "preview_cards_statuses",
                columns: new[] { "status_id", "preview_card_id" });

            migrationBuilder.CreateIndex(
                name: "index_report_notes_on_account_id",
                table: "report_notes",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "index_report_notes_on_report_id",
                table: "report_notes",
                column: "report_id");

            migrationBuilder.CreateIndex(
                name: "index_reports_on_account_id",
                table: "reports",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "index_reports_on_action_taken_by_account_id",
                table: "reports",
                column: "action_taken_by_account_id",
                filter: "(action_taken_by_account_id IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "index_reports_on_assigned_account_id",
                table: "reports",
                column: "assigned_account_id",
                filter: "(assigned_account_id IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "index_reports_on_target_account_id",
                table: "reports",
                column: "target_account_id");

            migrationBuilder.CreateIndex(
                name: "index_scheduled_statuses_on_account_id",
                table: "scheduled_statuses",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "index_scheduled_statuses_on_scheduled_at",
                table: "scheduled_statuses",
                column: "scheduled_at");

            migrationBuilder.CreateIndex(
                name: "index_session_activations_on_access_token_id",
                table: "session_activations",
                column: "access_token_id");

            migrationBuilder.CreateIndex(
                name: "index_session_activations_on_session_id",
                table: "session_activations",
                column: "session_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_session_activations_on_user_id",
                table: "session_activations",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "index_settings_on_thing_type_and_thing_id_and_var",
                table: "settings",
                columns: new[] { "thing_type", "thing_id", "var" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_site_uploads_on_var",
                table: "site_uploads",
                column: "var",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_status_edits_on_account_id",
                table: "status_edits",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "index_status_edits_on_status_id",
                table: "status_edits",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "index_status_pins_on_account_id_and_status_id",
                table: "status_pins",
                columns: new[] { "account_id", "status_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_status_pins_on_status_id",
                table: "status_pins",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "index_status_stats_on_status_id",
                table: "status_stats",
                column: "status_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_status_trends_on_account_id",
                table: "status_trends",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "index_status_trends_on_status_id",
                table: "status_trends",
                column: "status_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_statuses_20190820",
                table: "statuses",
                columns: new[] { "account_id", "id", "visibility", "updated_at" },
                descending: new[] { false, true, false, false },
                filter: "(deleted_at IS NULL)");

            migrationBuilder.CreateIndex(
                name: "index_statuses_on_account_id",
                table: "statuses",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "index_statuses_on_deleted_at",
                table: "statuses",
                column: "deleted_at",
                filter: "(deleted_at IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "index_statuses_on_in_reply_to_account_id",
                table: "statuses",
                column: "in_reply_to_account_id",
                filter: "(in_reply_to_account_id IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "index_statuses_on_in_reply_to_id",
                table: "statuses",
                column: "in_reply_to_id",
                filter: "(in_reply_to_id IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "index_statuses_on_reblog_of_id_and_account_id",
                table: "statuses",
                columns: new[] { "reblog_of_id", "account_id" });

            migrationBuilder.CreateIndex(
                name: "index_statuses_on_uri",
                table: "statuses",
                column: "uri",
                unique: true,
                filter: "(uri IS NOT NULL)")
                .Annotation("Npgsql:IndexOperators", new[] { "text_pattern_ops" });

            migrationBuilder.CreateIndex(
                name: "index_statuses_public_20200119",
                table: "statuses",
                columns: new[] { "id", "account_id" },
                descending: new[] { true, false },
                filter: "((deleted_at IS NULL) AND (visibility = 0) AND (reblog_of_id IS NULL) AND ((NOT reply) OR (in_reply_to_account_id = account_id)))");

            migrationBuilder.CreateIndex(
                name: "index_statuses_tags_on_status_id",
                table: "statuses_tags",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "index_statuses_tags_on_tag_id_and_status_id",
                table: "statuses_tags",
                columns: new[] { "tag_id", "status_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_tag_follows_on_account_id_and_tag_id",
                table: "tag_follows",
                columns: new[] { "account_id", "tag_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_tag_follows_on_tag_id",
                table: "tag_follows",
                column: "tag_id");

            migrationBuilder.CreateIndex(
                name: "index_tombstones_on_account_id",
                table: "tombstones",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "index_tombstones_on_uri",
                table: "tombstones",
                column: "uri");

            migrationBuilder.CreateIndex(
                name: "index_unavailable_domains_on_domain",
                table: "unavailable_domains",
                column: "domain",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_user_invite_requests_on_user_id",
                table: "user_invite_requests",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "index_users_on_account_id",
                table: "users",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "index_users_on_confirmation_token",
                table: "users",
                column: "confirmation_token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_users_on_created_by_application_id",
                table: "users",
                column: "created_by_application_id",
                filter: "(created_by_application_id IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "index_users_on_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_users_on_reset_password_token",
                table: "users",
                column: "reset_password_token",
                unique: true,
                filter: "(reset_password_token IS NOT NULL)")
                .Annotation("Npgsql:IndexOperators", new[] { "text_pattern_ops" });

            migrationBuilder.CreateIndex(
                name: "index_users_on_role_id",
                table: "users",
                column: "role_id",
                filter: "(role_id IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "index_web_push_subscriptions_on_access_token_id",
                table: "web_push_subscriptions",
                column: "access_token_id",
                filter: "(access_token_id IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "index_web_push_subscriptions_on_user_id",
                table: "web_push_subscriptions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "index_web_settings_on_user_id",
                table: "web_settings",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_webauthn_credentials_on_external_id",
                table: "webauthn_credentials",
                column: "external_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "index_webauthn_credentials_on_user_id",
                table: "webauthn_credentials",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "index_webhooks_on_url",
                table: "webhooks",
                column: "url",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_rails_096669d221",
                table: "backups",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_rails_393f74df68",
                table: "devices",
                column: "access_token_id",
                principalTable: "oauth_access_tokens",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_bea040f377",
                table: "identities",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_rails_ff69dbb2ac",
                table: "invites",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_rails_e4b6396b41",
                table: "login_activities",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_rails_a7009bc2b6",
                table: "markers",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_34d54b0a33",
                table: "oauth_access_grants",
                column: "application_id",
                principalTable: "oauth_applications",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_63b044929b",
                table: "oauth_access_grants",
                column: "resource_owner_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_e84df68546",
                table: "oauth_access_tokens",
                column: "resource_owner_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_f5fc4c1ee3",
                table: "oauth_access_tokens",
                column: "application_id",
                principalTable: "oauth_applications",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_b0988c7c0a",
                table: "oauth_applications",
                column: "owner_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.Sql(UserIpsView);
            
            migrationBuilder.Sql(AccountSummariesView);
            
            migrationBuilder.Sql(FollowRecommendationsView);
            
            migrationBuilder.Sql(InstancesView);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP MATERIALIZED VIEW instances;");
            
            migrationBuilder.Sql("DROP MATERIALIZED VIEW follow_recommendations;");
            
            migrationBuilder.Sql("DROP MATERIALIZED VIEW account_summaries;");
            
            migrationBuilder.Sql("DROP VIEW user_ips;");
            
            migrationBuilder.DropForeignKey(
                name: "fk_50500f500d",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "fk_rails_ff69dbb2ac",
                table: "invites");

            migrationBuilder.DropForeignKey(
                name: "fk_b0988c7c0a",
                table: "oauth_applications");

            migrationBuilder.DropTable(
                name: "account_aliases");

            migrationBuilder.DropTable(
                name: "account_conversations");

            migrationBuilder.DropTable(
                name: "account_deletion_requests");

            migrationBuilder.DropTable(
                name: "account_domain_blocks");

            migrationBuilder.DropTable(
                name: "account_migrations");

            migrationBuilder.DropTable(
                name: "account_moderation_notes");

            migrationBuilder.DropTable(
                name: "account_notes");

            migrationBuilder.DropTable(
                name: "account_pins");

            migrationBuilder.DropTable(
                name: "account_stats");

            migrationBuilder.DropTable(
                name: "account_statuses_cleanup_policies");

            migrationBuilder.DropTable(
                name: "account_warning_presets");

            migrationBuilder.DropTable(
                name: "accounts_tags");

            migrationBuilder.DropTable(
                name: "admin_action_logs");

            migrationBuilder.DropTable(
                name: "announcement_mutes");

            migrationBuilder.DropTable(
                name: "announcement_reactions");

            migrationBuilder.DropTable(
                name: "appeals");

            migrationBuilder.DropTable(
                name: "ar_internal_metadata");

            migrationBuilder.DropTable(
                name: "backups");

            migrationBuilder.DropTable(
                name: "blocks");

            migrationBuilder.DropTable(
                name: "bookmarks");

            migrationBuilder.DropTable(
                name: "canonical_email_blocks");

            migrationBuilder.DropTable(
                name: "conversation_mutes");

            migrationBuilder.DropTable(
                name: "custom_emoji_categories");

            migrationBuilder.DropTable(
                name: "custom_filter_keywords");

            migrationBuilder.DropTable(
                name: "custom_filter_statuses");

            migrationBuilder.DropTable(
                name: "domain_allows");

            migrationBuilder.DropTable(
                name: "domain_blocks");

            migrationBuilder.DropTable(
                name: "email_domain_blocks");

            migrationBuilder.DropTable(
                name: "encrypted_messages");

            migrationBuilder.DropTable(
                name: "favourites");

            migrationBuilder.DropTable(
                name: "featured_tags");

            migrationBuilder.DropTable(
                name: "follow_recommendation_suppressions");

            migrationBuilder.DropTable(
                name: "follow_requests");

            migrationBuilder.DropTable(
                name: "identities");

            migrationBuilder.DropTable(
                name: "imports");

            migrationBuilder.DropTable(
                name: "ip_blocks");

            migrationBuilder.DropTable(
                name: "list_accounts");

            migrationBuilder.DropTable(
                name: "login_activities");

            migrationBuilder.DropTable(
                name: "markers");

            migrationBuilder.DropTable(
                name: "media_attachments");

            migrationBuilder.DropTable(
                name: "mentions");

            migrationBuilder.DropTable(
                name: "mutes");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "oauth_access_grants");

            migrationBuilder.DropTable(
                name: "one_time_keys");

            migrationBuilder.DropTable(
                name: "pghero_space_stats");

            migrationBuilder.DropTable(
                name: "poll_votes");

            migrationBuilder.DropTable(
                name: "preview_card_providers");

            migrationBuilder.DropTable(
                name: "preview_card_trends");

            migrationBuilder.DropTable(
                name: "preview_cards_statuses");

            migrationBuilder.DropTable(
                name: "relays");

            migrationBuilder.DropTable(
                name: "report_notes");

            migrationBuilder.DropTable(
                name: "rules");

            migrationBuilder.DropTable(
                name: "session_activations");

            migrationBuilder.DropTable(
                name: "settings");

            migrationBuilder.DropTable(
                name: "site_uploads");

            migrationBuilder.DropTable(
                name: "status_edits");

            migrationBuilder.DropTable(
                name: "status_pins");

            migrationBuilder.DropTable(
                name: "status_stats");

            migrationBuilder.DropTable(
                name: "status_trends");

            migrationBuilder.DropTable(
                name: "statuses_tags");

            migrationBuilder.DropTable(
                name: "system_keys");

            migrationBuilder.DropTable(
                name: "tag_follows");

            migrationBuilder.DropTable(
                name: "tombstones");

            migrationBuilder.DropTable(
                name: "unavailable_domains");

            migrationBuilder.DropTable(
                name: "user_invite_requests");

            migrationBuilder.DropTable(
                name: "web_push_subscriptions");

            migrationBuilder.DropTable(
                name: "web_settings");

            migrationBuilder.DropTable(
                name: "webauthn_credentials");

            migrationBuilder.DropTable(
                name: "webhooks");

            migrationBuilder.DropTable(
                name: "announcements");

            migrationBuilder.DropTable(
                name: "custom_emojis");

            migrationBuilder.DropTable(
                name: "account_warnings");

            migrationBuilder.DropTable(
                name: "conversations");

            migrationBuilder.DropTable(
                name: "custom_filters");

            migrationBuilder.DropTable(
                name: "follows");

            migrationBuilder.DropTable(
                name: "lists");

            migrationBuilder.DropTable(
                name: "scheduled_statuses");

            migrationBuilder.DropTable(
                name: "devices");

            migrationBuilder.DropTable(
                name: "polls");

            migrationBuilder.DropTable(
                name: "preview_cards");

            migrationBuilder.DropTable(
                name: "tags");

            migrationBuilder.DropTable(
                name: "reports");

            migrationBuilder.DropTable(
                name: "oauth_access_tokens");

            migrationBuilder.DropTable(
                name: "statuses");

            migrationBuilder.DropTable(
                name: "accounts");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "invites");

            migrationBuilder.DropTable(
                name: "oauth_applications");

            migrationBuilder.DropSequence(
                name: "accounts_id_seq");

            migrationBuilder.DropSequence(
                name: "encrypted_messages_id_seq");

            migrationBuilder.DropSequence(
                name: "media_attachments_id_seq");

            migrationBuilder.DropSequence(
                name: "statuses_id_seq");

            migrationBuilder.Sql("drop function timestamp_id(text)");
        }
    }
}
