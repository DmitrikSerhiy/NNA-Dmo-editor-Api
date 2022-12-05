﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NNA.Persistence;

#nullable disable

namespace Persistence.Migrations
{
    [DbContext(typeof(NnaContext))]
    [Migration("20221205131457_AddConflictOrder")]
    partial class AddConflictOrder
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);

                    b.HasDiscriminator<string>("Discriminator").HasValue("IdentityUserLogin<Guid>");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);

                    b.HasDiscriminator<string>("Discriminator").HasValue("IdentityUserToken<Guid>");
                });

            modelBuilder.Entity("NNA.Domain.Entities.Beat", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("BeatTime")
                        .HasColumnType("int");

                    b.Property<string>("BeatTimeView")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("DateOfCreation")
                        .HasColumnType("bigint");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("DmoId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Order")
                        .HasColumnType("int");

                    b.Property<string>("TempId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("DmoId");

                    b.HasIndex("UserId");

                    b.ToTable("Beats");
                });

            modelBuilder.Entity("NNA.Domain.Entities.Dmo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("BeatsJson")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ControllingIdea")
                        .HasColumnType("nvarchar(max)");

                    b.Property<short>("ControllingIdeaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint")
                        .HasDefaultValue((short)0);

                    b.Property<long>("DateOfCreation")
                        .HasColumnType("bigint");

                    b.Property<bool?>("Didacticism")
                        .HasColumnType("bit");

                    b.Property<string>("DidacticismDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<short>("DmoStatus")
                        .HasColumnType("smallint");

                    b.Property<bool>("HasBeats")
                        .HasColumnType("bit");

                    b.Property<short?>("Mark")
                        .HasColumnType("smallint");

                    b.Property<string>("MovieTitle")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("NnaUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Premise")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ShortComment")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("NnaUserId");

                    b.ToTable("Dmos");
                });

            modelBuilder.Entity("NNA.Domain.Entities.DmoCollection", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CollectionName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("DateOfCreation")
                        .HasColumnType("bigint");

                    b.Property<Guid>("NnaUserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("NnaUserId");

                    b.ToTable("DmoCollections");
                });

            modelBuilder.Entity("NNA.Domain.Entities.DmoCollectionDmo", b =>
                {
                    b.Property<Guid>("DmoId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("DmoCollectionId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("DmoId", "DmoCollectionId");

                    b.HasIndex("DmoCollectionId");

                    b.ToTable("DmoCollectionDmo");
                });

            modelBuilder.Entity("NNA.Domain.Entities.EditorConnection", b =>
                {
                    b.Property<string>("ConnectionId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("ConnectionId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("EditorConnections");
                });

            modelBuilder.Entity("NNA.Domain.Entities.NnaMovieCharacter", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Aliases")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("CharacterContradictsCharacterization")
                        .HasColumnType("bit");

                    b.Property<string>("CharacterContradictsCharacterizationDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Characterization")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Color")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(7)
                        .HasColumnType("nvarchar(7)")
                        .HasDefaultValue("#000000");

                    b.Property<long>("DateOfCreation")
                        .HasColumnType("bigint");

                    b.Property<Guid>("DmoId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Emphathetic")
                        .HasColumnType("bit");

                    b.Property<string>("EmphatheticDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Goal")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Sympathetic")
                        .HasColumnType("bit");

                    b.Property<string>("SympatheticDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UnconsciousGoal")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("DmoId");

                    b.ToTable("Characters");
                });

            modelBuilder.Entity("NNA.Domain.Entities.NnaMovieCharacterConflictInDmo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Achieved")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<Guid>("CharacterId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<short>("CharacterType")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint")
                        .HasDefaultValue((short)1);

                    b.Property<long>("DateOfCreation")
                        .HasColumnType("bigint");

                    b.Property<int>("PairOrder")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CharacterId");

                    b.ToTable("NnaMovieCharacterConflicts");
                });

            modelBuilder.Entity("NNA.Domain.Entities.NnaMovieCharacterInBeat", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("BeatId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CharacterId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<long>("DateOfCreation")
                        .HasColumnType("bigint");

                    b.Property<string>("TempId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("BeatId");

                    b.HasIndex("CharacterId");

                    b.ToTable("CharacterInBeats");
                });

            modelBuilder.Entity("NNA.Domain.Entities.NnaRole", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("NNA.Domain.Entities.NnaUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("AuthProviders")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("NNA.Domain.Entities.UsersTokens", b =>
                {
                    b.Property<string>("AccessTokenId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LoginProvider")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RefreshTokenId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.ToView("UsersTokens");
                });

            modelBuilder.Entity("NNA.Domain.Entities.NnaLogin", b =>
                {
                    b.HasBaseType("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>");

                    b.HasDiscriminator().HasValue("NnaLogin");
                });

            modelBuilder.Entity("NNA.Domain.Entities.NnaToken", b =>
                {
                    b.HasBaseType("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>");

                    b.Property<string>("TokenKeyId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue("NnaToken");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.HasOne("NNA.Domain.Entities.NnaRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.HasOne("NNA.Domain.Entities.NnaUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.HasOne("NNA.Domain.Entities.NnaUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.HasOne("NNA.Domain.Entities.NnaRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("NNA.Domain.Entities.NnaUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.HasOne("NNA.Domain.Entities.NnaUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("NNA.Domain.Entities.Beat", b =>
                {
                    b.HasOne("NNA.Domain.Entities.Dmo", "Dmo")
                        .WithMany("Beats")
                        .HasForeignKey("DmoId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("NNA.Domain.Entities.NnaUser", "User")
                        .WithMany("Beats")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Dmo");

                    b.Navigation("User");
                });

            modelBuilder.Entity("NNA.Domain.Entities.Dmo", b =>
                {
                    b.HasOne("NNA.Domain.Entities.NnaUser", "NnaUser")
                        .WithMany("Dmos")
                        .HasForeignKey("NnaUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("NnaUser");
                });

            modelBuilder.Entity("NNA.Domain.Entities.DmoCollection", b =>
                {
                    b.HasOne("NNA.Domain.Entities.NnaUser", "NnaUser")
                        .WithMany("DmoCollections")
                        .HasForeignKey("NnaUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("NnaUser");
                });

            modelBuilder.Entity("NNA.Domain.Entities.DmoCollectionDmo", b =>
                {
                    b.HasOne("NNA.Domain.Entities.DmoCollection", "DmoCollection")
                        .WithMany("DmoCollectionDmos")
                        .HasForeignKey("DmoCollectionId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("NNA.Domain.Entities.Dmo", "Dmo")
                        .WithMany("DmoCollectionDmos")
                        .HasForeignKey("DmoId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Dmo");

                    b.Navigation("DmoCollection");
                });

            modelBuilder.Entity("NNA.Domain.Entities.NnaMovieCharacter", b =>
                {
                    b.HasOne("NNA.Domain.Entities.Dmo", "Dmo")
                        .WithMany("Characters")
                        .HasForeignKey("DmoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Dmo");
                });

            modelBuilder.Entity("NNA.Domain.Entities.NnaMovieCharacterConflictInDmo", b =>
                {
                    b.HasOne("NNA.Domain.Entities.NnaMovieCharacter", "Character")
                        .WithMany("Conflicts")
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Character");
                });

            modelBuilder.Entity("NNA.Domain.Entities.NnaMovieCharacterInBeat", b =>
                {
                    b.HasOne("NNA.Domain.Entities.Beat", "Beat")
                        .WithMany("Characters")
                        .HasForeignKey("BeatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("NNA.Domain.Entities.NnaMovieCharacter", "Character")
                        .WithMany("Beats")
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Beat");

                    b.Navigation("Character");
                });

            modelBuilder.Entity("NNA.Domain.Entities.Beat", b =>
                {
                    b.Navigation("Characters");
                });

            modelBuilder.Entity("NNA.Domain.Entities.Dmo", b =>
                {
                    b.Navigation("Beats");

                    b.Navigation("Characters");

                    b.Navigation("DmoCollectionDmos");
                });

            modelBuilder.Entity("NNA.Domain.Entities.DmoCollection", b =>
                {
                    b.Navigation("DmoCollectionDmos");
                });

            modelBuilder.Entity("NNA.Domain.Entities.NnaMovieCharacter", b =>
                {
                    b.Navigation("Beats");

                    b.Navigation("Conflicts");
                });

            modelBuilder.Entity("NNA.Domain.Entities.NnaUser", b =>
                {
                    b.Navigation("Beats");

                    b.Navigation("DmoCollections");

                    b.Navigation("Dmos");
                });
#pragma warning restore 612, 618
        }
    }
}
