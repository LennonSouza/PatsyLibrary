﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PatsyLibrary.Models;

namespace PatsyLibrary.Data.EntityConfiguration;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");

        // Configuração da chave primária
        builder.HasKey(p => p.PermissionId);

        // Configura o identity autoIncrement
        builder.Property(p => p.PermissionId)
               .ValueGeneratedNever();

        // Configuração das propriedades
        builder.Property(p => p.Name)
               .IsRequired()
               .HasMaxLength(255);  // Limite de tamanho da string, ajuste conforme necessário

        builder.Navigation(x => x.RolePermissions).AutoInclude();
    }
}