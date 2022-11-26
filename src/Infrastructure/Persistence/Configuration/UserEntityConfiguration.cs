using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        
        builder.HasKey(e => e.Id).HasName("users_pkey");

        builder.HasIndex(e => e.AccountId).HasDatabaseName("index_users_on_account_id");

        builder.HasIndex(e => e.ConfirmationToken)
            .HasDatabaseName("index_users_on_confirmation_token")
            .IsUnique();

        builder.HasIndex(e => e.CreatedByApplicationId)
            .HasDatabaseName("index_users_on_created_by_application_id")
            .HasFilter("(created_by_application_id IS NOT NULL)");

        builder.HasIndex(e => e.Email)
            .HasDatabaseName("index_users_on_email")
            .IsUnique();

        builder.HasIndex(e => e.ResetPasswordToken)
            .HasDatabaseName("index_users_on_reset_password_token")
            .IsUnique()
            .HasFilter("(reset_password_token IS NOT NULL)")
            .HasOperators(new[] { "text_pattern_ops" });

        builder.HasIndex(e => e.RoleId)
            .HasDatabaseName("index_users_on_role_id")
            .HasFilter("(role_id IS NOT NULL)");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.Property(e => e.Admin).HasColumnName("admin");

        builder.Property(e => e.Approved)
            .IsRequired()
            .HasColumnName("approved")
            .HasDefaultValueSql("true");

        builder.Property(e => e.ChosenLanguages)
            .HasColumnType("character varying[]")
            .HasColumnName("chosen_languages");

        builder.Property(e => e.ConfirmationSentAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("confirmation_sent_at");

        builder.Property(e => e.ConfirmationToken)
            .HasColumnType("character varying")
            .HasColumnName("confirmation_token");

        builder.Property(e => e.ConfirmedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("confirmed_at");

        builder.Property(e => e.ConsumedTimestep).HasColumnName("consumed_timestep");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.CreatedByApplicationId).HasColumnName("created_by_application_id");

        builder.Property(e => e.CurrentSignInAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("current_sign_in_at");

        builder.Property(e => e.Disabled).HasColumnName("disabled");

        builder.Property(e => e.Email)
            .HasColumnType("character varying")
            .HasColumnName("email")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.EncryptedOtpSecret)
            .HasColumnType("character varying")
            .HasColumnName("encrypted_otp_secret");

        builder.Property(e => e.EncryptedOtpSecretIv)
            .HasColumnType("character varying")
            .HasColumnName("encrypted_otp_secret_iv");

        builder.Property(e => e.EncryptedOtpSecretSalt)
            .HasColumnType("character varying")
            .HasColumnName("encrypted_otp_secret_salt");

        builder.Property(e => e.EncryptedPassword)
            .HasColumnType("character varying")
            .HasColumnName("encrypted_password")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.InviteId).HasColumnName("invite_id");

        builder.Property(e => e.LastEmailedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("last_emailed_at");

        builder.Property(e => e.LastSignInAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("last_sign_in_at");

        builder.Property(e => e.Locale)
            .HasColumnType("character varying")
            .HasColumnName("locale");

        builder.Property(e => e.Moderator).HasColumnName("moderator");

        builder.Property(e => e.OtpBackupCodes)
            .HasColumnType("character varying[]")
            .HasColumnName("otp_backup_codes");

        builder.Property(e => e.OtpRequiredForLogin).HasColumnName("otp_required_for_login");

        builder.Property(e => e.ResetPasswordSentAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("reset_password_sent_at");

        builder.Property(e => e.ResetPasswordToken)
            .HasColumnType("character varying")
            .HasColumnName("reset_password_token");

        builder.Property(e => e.RoleId).HasColumnName("role_id");

        builder.Property(e => e.SignInCount).HasColumnName("sign_in_count");

        builder.Property(e => e.SignInToken)
            .HasColumnType("character varying")
            .HasColumnName("sign_in_token");

        builder.Property(e => e.SignInTokenSentAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("sign_in_token_sent_at");

        builder.Property(e => e.SignUpIp).HasColumnName("sign_up_ip");

        builder.Property(e => e.SkipSignInToken).HasColumnName("skip_sign_in_token");

        builder.Property(e => e.UnconfirmedEmail)
            .HasColumnType("character varying")
            .HasColumnName("unconfirmed_email");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.Property(e => e.WebAuthnId)
            .HasColumnType("character varying")
            .HasColumnName("webauthn_id");

        builder.HasOne(d => d.Account)
            .WithMany(p => p.Users)
            .HasForeignKey(d => d.AccountId)
            .HasConstraintName("fk_50500f500d");

        builder.HasOne(d => d.CreatedByApplication)
            .WithMany(p => p.Users)
            .HasForeignKey(d => d.CreatedByApplicationId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("fk_rails_ecc9536e7c");

        builder.HasOne(d => d.Invite)
            .WithMany(p => p.Users)
            .HasForeignKey(d => d.InviteId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("fk_rails_8fb2a43e88");

        builder.HasOne(d => d.Role)
            .WithMany(p => p.Users)
            .HasForeignKey(d => d.RoleId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("fk_rails_642f17018b");
    }
}