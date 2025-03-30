﻿using Microsoft.EntityFrameworkCore;
using PatsyLibrary.Contracts.DataAccess.Interfaces;
using PatsyLibrary.Models;

namespace PatsyLibrary.Services;

public static class DefaultPermissions
{
    private static List<Permission> _systemPermissionsCache;

    public static readonly Dictionary<short, string> SystemPermissions = new()
    {
        { 000, "Super Admin" },      // 0XX para Super Admin
        { 100, "Ver Permissões" },   // 1XX para Permissões
        { 101, "Adicionar Permissões" },   
        { 102, "Editar Permissões" }, 
        { 103, "Excluir Permissões" }, 
        { 200, "Ver Departamentos" }, // 2XX para Departamentos
        { 201, "Adicionar Departamentos" },
        { 202, "Editar Departamentos" },
        { 203, "Excluir Departamentos" },
        { 300, "Ver Cargos" },       // 3XX para Cargos (Roles)
        { 301, "Adicionar Cargos" },
        { 302, "Editar Cargos" },
        { 303, "Excluir Cargos" },
        { 400, "Ver Usuarios" },     // 4XX para Usuários
        { 401, "Adicionar Usuarios" },
        { 402, "Editar Usuarios" }, 
        { 403, "Excluir Usuarios" }
    };

    public static async Task Initialize(IUnitOfWorkRepository unitOfWork) 
        => _systemPermissionsCache = await unitOfWork.PermissionRepository
            .GetAll
            .Where(p => SystemPermissions.Values.Contains(p.Name))
            .ToListAsync();

    public static bool IsSystemPermission(short permissionId) => _systemPermissionsCache.Any(p => p.PermissionId == permissionId);

    public static bool IsSystemPermission(string name) => SystemPermissions.Values.Contains(name);
}