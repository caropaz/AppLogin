using AppLogin.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace AppLogin.Context
{
    public class AppDBContext: DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options)
        :   base(options) { }

        public DbSet<Usuario> Usuarios => Set<Usuario>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Usuario>(table => 
            {
                table.Property(col => col.Username).HasMaxLength(20);
                table.Property(col => col.Name).HasMaxLength(20);
                table.Property(col => col.Password).HasMaxLength(12);
            });
        }
    }
}
