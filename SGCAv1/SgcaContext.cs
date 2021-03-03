using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace SGCAv1
{
    public partial class SgcaContext : DbContext
    {
        public SgcaContext()
        {
        }

        public SgcaContext(DbContextOptions<SgcaContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Caixa> Caixas { get; set; }
        public virtual DbSet<Notum> Nota { get; set; }
        public virtual DbSet<Sgca> Sgcas { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=.\\SQLEXPRESS; Initial Catalog=Sgca; Integrated Security=SSPI");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Latin1_General_CI_AS");

            modelBuilder.Entity<Caixa>(entity =>
            {
                entity.ToTable("Caixa");

                entity.Property(e => e.CaixaSituacao)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Notum>(entity =>
            {
                entity.HasKey(e => e.NotaId)
                    .HasName("PK__Nota__EF36CC1A0CBD6F83");
            });

            modelBuilder.Entity<Sgca>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("SGCA");

                entity.HasOne(d => d.Caixa)
                    .WithMany()
                    .HasForeignKey(d => d.CaixaId)
                    .HasConstraintName("FK__SGCA__CaixaId__38996AB5");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
