using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HotelCostaAzul.API.Models;

public partial class HotelDbContext : DbContext
{
    public HotelDbContext()
    {
    }

    public HotelDbContext(DbContextOptions<HotelDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Comentario> Comentarios { get; set; }

    public virtual DbSet<DetalleFactura> DetalleFacturas { get; set; }

    public virtual DbSet<Factura> Facturas { get; set; }

    public virtual DbSet<Habitacione> Habitaciones { get; set; }

    public virtual DbSet<Hotele> Hoteles { get; set; }

    public virtual DbSet<MetodosPago> MetodosPagos { get; set; }

    public virtual DbSet<Pago> Pagos { get; set; }

    public virtual DbSet<Reserva> Reservas { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-FANLTA8;Database=HotelCostaAzul;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comentario>(entity =>
        {
            entity.Property(e => e.Calificacion).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Comentario1).HasColumnName("Comentario");
        });

        modelBuilder.Entity<DetalleFactura>(entity =>
        {
            entity.Property(e => e.Monto).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PrecioUni).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SubTotal).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Factura>(entity =>
        {
            entity.Property(e => e.Estado).HasMaxLength(150);
            entity.Property(e => e.MontoTotal).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Habitacione>(entity =>
        {
            entity.Property(e => e.PrecioPersona).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Hotele>(entity =>
        {
            entity.Property(e => e.Calificacion).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Ciudad).HasMaxLength(150);
            entity.Property(e => e.NombreHotel).HasMaxLength(200);
            entity.Property(e => e.Pais).HasMaxLength(150);
        });

        modelBuilder.Entity<MetodosPago>(entity =>
        {
            entity.Property(e => e.IdUsuario)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Tipo).HasMaxLength(50);
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.Property(e => e.MontoTotal).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.Property(e => e.Monto).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.NombreUsu).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
