using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace UnphuCard_Api.Models;

public partial class UnphuCardContext : DbContext
{
    public UnphuCardContext()
    {
    }

    public UnphuCardContext(DbContextOptions<UnphuCardContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Acceso> Accesos { get; set; }

    public virtual DbSet<Aula> Aulas { get; set; }

    public virtual DbSet<Carrito> Carritos { get; set; }

    public virtual DbSet<CategoriaProducto> CategoriaProductos { get; set; }

    public virtual DbSet<Compra> Compras { get; set; }

    public virtual DbSet<DetallesCompra> DetallesCompras { get; set; }

    public virtual DbSet<Establecimiento> Establecimientos { get; set; }

    public virtual DbSet<Estado> Estados { get; set; }

    public virtual DbSet<Horario> Horarios { get; set; }

    public virtual DbSet<Inscripcione> Inscripciones { get; set; }

    public virtual DbSet<Inventario> Inventarios { get; set; }

    public virtual DbSet<Materia> Materias { get; set; }

    public virtual DbSet<MetodoPago> MetodoPagos { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Recarga> Recargas { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolesPermiso> RolesPermisos { get; set; }

    public virtual DbSet<Sesion> Sesions { get; set; }

    public virtual DbSet<Tarjeta> Tarjetas { get; set; }

    public virtual DbSet<TarjetasProvisionale> TarjetasProvisionales { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<VwAccesosUsuario> VwAccesosUsuarios { get; set; }

    public virtual DbSet<VwCarritoCompra> VwCarritoCompras { get; set; }

    public virtual DbSet<VwComprasUsuario> VwComprasUsuarios { get; set; }

    public virtual DbSet<VwInventarioEstablecimiento> VwInventarioEstablecimientos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=tcp:unphucard.database.windows.net,1433;Initial Catalog=UnphuCard;Persist Security Info=False;User ID=UnphuCard;Password=Proyecto1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Acceso>(entity =>
        {
            entity.HasKey(e => e.AccesId).HasName("PK__Accesos__722B185E162FC962");

            entity.Property(e => e.AccesId).HasColumnName("Acces_ID");
            entity.Property(e => e.AccesFecha)
                .HasColumnType("datetime")
                .HasColumnName("Acces_Fecha");
            entity.Property(e => e.AulaId).HasColumnName("Aula_ID");
            entity.Property(e => e.StatusId).HasColumnName("Status_ID");
            entity.Property(e => e.UsuId).HasColumnName("Usu_ID");
        });

        modelBuilder.Entity<Aula>(entity =>
        {
            entity.HasKey(e => e.AulaId).HasName("PK__Aulas__6A82480FCB64A6AB");

            entity.Property(e => e.AulaId).HasColumnName("Aula_ID");
            entity.Property(e => e.AulaDescripcion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Aula_Descripcion");
            entity.Property(e => e.AulaSensor)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Aula_Sensor");
            entity.Property(e => e.AulaUbicacion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Aula_Ubicacion");
        });

        modelBuilder.Entity<Carrito>(entity =>
        {
            entity.HasKey(e => e.CarId).HasName("PK__Carrito__523653D9381F0399");

            entity.ToTable("Carrito");

            entity.Property(e => e.CarId).HasColumnName("Car_ID");
            entity.Property(e => e.CarCantidad).HasColumnName("Car_Cantidad");
            entity.Property(e => e.CarFecha)
                .HasColumnType("datetime")
                .HasColumnName("Car_Fecha");
            entity.Property(e => e.ProdId).HasColumnName("Prod_ID");
            entity.Property(e => e.SesionId).HasColumnName("Sesion_ID");
        });

        modelBuilder.Entity<CategoriaProducto>(entity =>
        {
            entity.HasKey(e => e.CatProdId).HasName("PK__Categori__A9B442F26E41FAA0");

            entity.ToTable("CategoriaProducto");

            entity.Property(e => e.CatProdId).HasColumnName("CatProd_ID");
            entity.Property(e => e.CatProdDescripcion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CatProd_Descripcion");
        });

        modelBuilder.Entity<Compra>(entity =>
        {
            entity.HasKey(e => e.CompId).HasName("PK__Compras__DC0BCCC0E386A3B0");

            entity.Property(e => e.CompId).HasColumnName("Comp_ID");
            entity.Property(e => e.CompFecha)
                .HasColumnType("datetime")
                .HasColumnName("Comp_Fecha");
            entity.Property(e => e.CompMonto)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("Comp_Monto");
            entity.Property(e => e.EstId).HasColumnName("Est_ID");
            entity.Property(e => e.MetPagId).HasColumnName("MetPag_ID");
            entity.Property(e => e.SesionId).HasColumnName("Sesion_ID");
            entity.Property(e => e.UsuId).HasColumnName("Usu_ID");
        });

        modelBuilder.Entity<DetallesCompra>(entity =>
        {
            entity.HasKey(e => e.DetCompId).HasName("PK__Detalles__79BAB8C06B7111F4");

            entity.Property(e => e.DetCompId).HasColumnName("DetComp_ID");
            entity.Property(e => e.CompId).HasColumnName("Comp_ID");
            entity.Property(e => e.DetCompCantidad).HasColumnName("DetComp_Cantidad");
            entity.Property(e => e.DetCompPrecio)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("DetComp_Precio");
            entity.Property(e => e.ProdId).HasColumnName("Prod_ID");
            entity.Property(e => e.SesionId).HasColumnName("Sesion_ID");
        });

        modelBuilder.Entity<Establecimiento>(entity =>
        {
            entity.HasKey(e => e.EstId).HasName("PK__Establec__345473DC98FE74D8");

            entity.Property(e => e.EstId).HasColumnName("Est_ID");
            entity.Property(e => e.EstDescripcion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Est_Descripcion");
            entity.Property(e => e.EstUbicacion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Est_Ubicacion");
            entity.Property(e => e.UsuId).HasColumnName("Usu_ID");
        });

        modelBuilder.Entity<Estado>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__Estados__519009ACD08146D9");

            entity.Property(e => e.StatusId).HasColumnName("Status_ID");
            entity.Property(e => e.StatusDescripcion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Status_Descripcion");
        });

        modelBuilder.Entity<Horario>(entity =>
        {
            entity.HasKey(e => e.HorId).HasName("PK__Horarios__6B9DF12FD35A1768");

            entity.Property(e => e.HorId).HasColumnName("Hor_ID");
            entity.Property(e => e.AulaId).HasColumnName("Aula_ID");
            entity.Property(e => e.HorDia)
                .HasMaxLength(9)
                .IsUnicode(false)
                .HasColumnName("Hor_Dia");
            entity.Property(e => e.HorHoraFin).HasColumnName("Hor_HoraFin");
            entity.Property(e => e.HorHoraInicio).HasColumnName("Hor_HoraInicio");
            entity.Property(e => e.MatId).HasColumnName("Mat_ID");
            entity.Property(e => e.UsuId).HasColumnName("Usu_ID");
        });

        modelBuilder.Entity<Inscripcione>(entity =>
        {
            entity.HasKey(e => e.InsId).HasName("PK__Inscripc__151409CD8731CEE0");

            entity.HasIndex(e => new { e.UsuId, e.MatId, e.InsCuatrimestre }, "UQ__Inscripc__1481B5B7932C8A6C").IsUnique();

            entity.Property(e => e.InsId).HasColumnName("Ins_ID");
            entity.Property(e => e.InsCuatrimestre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Ins_Cuatrimestre");
            entity.Property(e => e.MatId).HasColumnName("Mat_ID");
            entity.Property(e => e.StatusId).HasColumnName("Status_ID");
            entity.Property(e => e.UsuId).HasColumnName("Usu_ID");
        });

        modelBuilder.Entity<Inventario>(entity =>
        {
            entity.HasKey(e => e.InvId).HasName("PK__Inventar__FCAFBC7B6380AA6E");

            entity.ToTable("Inventario");

            entity.Property(e => e.InvId).HasColumnName("Inv_ID");
            entity.Property(e => e.EstId).HasColumnName("Est_ID");
            entity.Property(e => e.InvCantidad).HasColumnName("Inv_Cantidad");
            entity.Property(e => e.InvFecha)
                .HasColumnType("datetime")
                .HasColumnName("Inv_Fecha");
            entity.Property(e => e.ProdId).HasColumnName("Prod_ID");
        });

        modelBuilder.Entity<Materia>(entity =>
        {
            entity.HasKey(e => e.MatId).HasName("PK__Materias__84D8705686A515F6");

            entity.Property(e => e.MatId).HasColumnName("Mat_ID");
            entity.Property(e => e.MatCodigo)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("Mat_Codigo");
            entity.Property(e => e.MatDescripcion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Mat_Descripcion");
            entity.Property(e => e.UsuId).HasColumnName("Usu_ID");
        });

        modelBuilder.Entity<MetodoPago>(entity =>
        {
            entity.HasKey(e => e.MetPagId).HasName("PK__MetodoPa__14BD9C0465D76B8F");

            entity.ToTable("MetodoPago");

            entity.Property(e => e.MetPagId).HasColumnName("MetPag_ID");
            entity.Property(e => e.MetPagDescripcion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("MetPag_Descripcion");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.ProdId).HasName("PK__Producto__C55BDFF3765C6F26");

            entity.ToTable("Producto");

            entity.Property(e => e.ProdId).HasColumnName("Prod_ID");
            entity.Property(e => e.CatProdId).HasColumnName("CatProd_ID");
            entity.Property(e => e.ProdDescripcion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Prod_Descripcion");
            entity.Property(e => e.ProdImagenes)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Prod_Imagenes");
            entity.Property(e => e.ProdPrecio)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("Prod_Precio");
            entity.Property(e => e.StatusId).HasColumnName("Status_ID");
        });

        modelBuilder.Entity<Recarga>(entity =>
        {
            entity.HasKey(e => e.RecId).HasName("PK__Recargas__81BCD9EA26F7A615");

            entity.Property(e => e.RecId).HasColumnName("Rec_ID");
            entity.Property(e => e.MetPagId).HasColumnName("MetPag_ID");
            entity.Property(e => e.RecFecha)
                .HasColumnType("datetime")
                .HasColumnName("Rec_Fecha");
            entity.Property(e => e.RecMonto)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("Rec_Monto");
            entity.Property(e => e.UsuId).HasColumnName("Usu_ID");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RolId).HasName("PK__Roles__795EBD6992DCDF07");

            entity.Property(e => e.RolId).HasColumnName("Rol_ID");
            entity.Property(e => e.RolDescripcion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Rol_Descripcion");
        });

        modelBuilder.Entity<RolesPermiso>(entity =>
        {
            entity.HasKey(e => e.RolPerId).HasName("PK__RolesPer__3F152B92025483D0");

            entity.Property(e => e.RolPerId).HasColumnName("RolPer_ID");
            entity.Property(e => e.RolId).HasColumnName("Rol_ID");
            entity.Property(e => e.RolPerDescripcion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("RolPer_Descripcion");
        });

        modelBuilder.Entity<Sesion>(entity =>
        {
            entity.HasKey(e => e.SesionId).HasName("PK__Sesion__39F29960DA934FF5");

            entity.ToTable("Sesion");

            entity.Property(e => e.SesionId).HasColumnName("Sesion_ID");
            entity.Property(e => e.EstId).HasColumnName("Est_ID");
            entity.Property(e => e.SesionFecha)
                .HasColumnType("datetime")
                .HasColumnName("Sesion_Fecha");
            entity.Property(e => e.SesionToken)
                .IsUnicode(false)
                .HasColumnName("Sesion_Token");
            entity.Property(e => e.UsuId).HasColumnName("Usu_ID");
        });

        modelBuilder.Entity<Tarjeta>(entity =>
        {
            entity.HasKey(e => e.TarjId).HasName("PK__Tarjetas__FF6850EEF833C603");

            entity.Property(e => e.TarjId).HasColumnName("Tarj_ID");
            entity.Property(e => e.StatusId).HasColumnName("Status_ID");
            entity.Property(e => e.TarjCodigo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Tarj_Codigo");
            entity.Property(e => e.TarjFecha)
                .HasColumnType("datetime")
                .HasColumnName("Tarj_Fecha");
            entity.Property(e => e.UsuId).HasColumnName("Usu_ID");
        });

        modelBuilder.Entity<TarjetasProvisionale>(entity =>
        {
            entity.HasKey(e => e.TarjProvId).HasName("PK__Tarjetas__29E2D84AC451BB11");

            entity.Property(e => e.TarjProvId).HasColumnName("TarjProv_ID");
            entity.Property(e => e.StatusId).HasColumnName("Status_ID");
            entity.Property(e => e.TarjProvCodigo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("TarjProv_Codigo");
            entity.Property(e => e.TarjProvFecha)
                .HasColumnType("datetime")
                .HasColumnName("TarjProv_Fecha");
            entity.Property(e => e.TarjProvFechaExpiracion)
                .HasColumnType("datetime")
                .HasColumnName("TarjProv_FechaExpiracion");
            entity.Property(e => e.UsuId).HasColumnName("Usu_ID");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.UsuId).HasName("PK__Usuarios__B6173FEB81514711");

            entity.HasIndex(e => e.UsuCodigo, "UQ__Usuarios__45605A21DEB730A9").IsUnique();

            entity.Property(e => e.UsuId).HasColumnName("Usu_ID");
            entity.Property(e => e.RolId).HasColumnName("Rol_ID");
            entity.Property(e => e.StatusId).HasColumnName("Status_ID");
            entity.Property(e => e.UsuApellido)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Usu_Apellido");
            entity.Property(e => e.UsuCampus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Usu_Campus");
            entity.Property(e => e.UsuCarrera)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Usu_Carrera");
            entity.Property(e => e.UsuCodigo).HasColumnName("Usu_Codigo");
            entity.Property(e => e.UsuContraseña)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Usu_Contraseña");
            entity.Property(e => e.UsuCorreo)
                .HasMaxLength(22)
                .IsUnicode(false)
                .HasColumnName("Usu_Correo");
            entity.Property(e => e.UsuMatricula)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasColumnName("Usu_Matricula");
            entity.Property(e => e.UsuNombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Usu_Nombre");
            entity.Property(e => e.UsuSaldo)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("Usu_Saldo");
            entity.Property(e => e.UsuUsuario)
                .HasMaxLength(9)
                .IsUnicode(false)
                .HasColumnName("Usu_Usuario");
        });

        modelBuilder.Entity<VwAccesosUsuario>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_AccesosUsuarios");

            entity.Property(e => e.Aula)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FechaDeIntento)
                .HasColumnType("datetime")
                .HasColumnName("Fecha de Intento");
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Matrícula)
                .HasMaxLength(7)
                .IsUnicode(false);
            entity.Property(e => e.NombreCompleto)
                .HasMaxLength(101)
                .IsUnicode(false)
                .HasColumnName("Nombre Completo");
        });

        modelBuilder.Entity<VwCarritoCompra>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_CarritoCompras");

            entity.Property(e => e.CantidadDeCompra).HasColumnName("Cantidad de Compra");
            entity.Property(e => e.FechaDeCompra)
                .HasColumnType("datetime")
                .HasColumnName("Fecha de Compra");
            entity.Property(e => e.IdDeCompra).HasColumnName("ID de Compra");
            entity.Property(e => e.IdDelProducto).HasColumnName("ID del Producto");
            entity.Property(e => e.NombreDelProducto)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Nombre del Producto");
            entity.Property(e => e.PrecioDelProducto)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("Precio del Producto");
            entity.Property(e => e.SesiónId).HasColumnName("Sesión ID");
        });

        modelBuilder.Entity<VwComprasUsuario>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_ComprasUsuarios");

            entity.Property(e => e.EstadoDelUsuario)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Estado del Usuario");
            entity.Property(e => e.FechaDeLaCompra)
                .HasColumnType("datetime")
                .HasColumnName("Fecha de la Compra");
            entity.Property(e => e.IdCompra).HasColumnName("ID Compra");
            entity.Property(e => e.Matrícula)
                .HasMaxLength(7)
                .IsUnicode(false);
            entity.Property(e => e.MontoDeCompra)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("Monto de Compra");
            entity.Property(e => e.MétodoDePago)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Método de Pago");
            entity.Property(e => e.NombreCompleto)
                .HasMaxLength(101)
                .IsUnicode(false)
                .HasColumnName("Nombre Completo");
            entity.Property(e => e.NombreDelEstablecimiento)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Nombre del Establecimiento");
        });

        modelBuilder.Entity<VwInventarioEstablecimiento>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_InventarioEstablecimientos");

            entity.Property(e => e.CantidadEnElInventario).HasColumnName("Cantidad en el Inventario");
            entity.Property(e => e.EstadoDelProducto)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Estado del Producto");
            entity.Property(e => e.FechaDeEntrada)
                .HasColumnType("datetime")
                .HasColumnName("Fecha de Entrada");
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.IdDelEstablecimiento).HasColumnName("ID del Establecimiento");
            entity.Property(e => e.IdDelProducto).HasColumnName("ID del Producto");
            entity.Property(e => e.ImagenDelProducto)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Imagen del Producto");
            entity.Property(e => e.NombreDelEstablecimiento)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Nombre del Establecimiento");
            entity.Property(e => e.NombreDelProducto)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Nombre del Producto");
            entity.Property(e => e.PrecioDelProducto)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("Precio del Producto");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
