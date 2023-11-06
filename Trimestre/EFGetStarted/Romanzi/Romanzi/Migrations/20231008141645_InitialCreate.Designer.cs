﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Romanzi.Data;

#nullable disable

namespace Romanzi.Migrations
{
    [DbContext(typeof(RomanziContext))]
    [Migration("20231008141645_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.11");

            modelBuilder.Entity("Romanzi.Model.Autore", b =>
                {
                    b.Property<int>("AutoreId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Cognome")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Nazionalità")
                        .HasColumnType("TEXT");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("AutoreId");

                    b.ToTable("Autori");
                });

            modelBuilder.Entity("Romanzi.Model.Personaggio", b =>
                {
                    b.Property<int>("PersonaggioId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("RomanzoId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Ruolo")
                        .HasColumnType("TEXT");

                    b.Property<string>("Sesso")
                        .HasColumnType("TEXT");

                    b.HasKey("PersonaggioId");

                    b.HasIndex("RomanzoId");

                    b.ToTable("Personaggi");
                });

            modelBuilder.Entity("Romanzi.Model.Romanzo", b =>
                {
                    b.Property<int>("RomanzoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("AnnoPubblicazione")
                        .HasColumnType("INTEGER");

                    b.Property<int>("AutoreId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Titolo")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("RomanzoId");

                    b.HasIndex("AutoreId");

                    b.ToTable("Romanzi");
                });

            modelBuilder.Entity("Romanzi.Model.Personaggio", b =>
                {
                    b.HasOne("Romanzi.Model.Romanzo", "Romanzo")
                        .WithMany("Personaggi")
                        .HasForeignKey("RomanzoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Romanzo");
                });

            modelBuilder.Entity("Romanzi.Model.Romanzo", b =>
                {
                    b.HasOne("Romanzi.Model.Autore", "Autore")
                        .WithMany("Romanzi")
                        .HasForeignKey("AutoreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Autore");
                });

            modelBuilder.Entity("Romanzi.Model.Autore", b =>
                {
                    b.Navigation("Romanzi");
                });

            modelBuilder.Entity("Romanzi.Model.Romanzo", b =>
                {
                    b.Navigation("Personaggi");
                });
#pragma warning restore 612, 618
        }
    }
}