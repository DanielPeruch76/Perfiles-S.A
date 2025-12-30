<%@ Page Title="Editar Departamento" Language="C#" MasterPageFile="~/Site.Master" 
    AutoEventWireup="true" CodeBehind="Editar.aspx.cs" 
    Inherits="PERFILES_SA.Pages.Departamentos.Editar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .info-card {
            background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
            border-left: 4px solid #007bff;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item">Departamentos</li>
    <li class="breadcrumb-item"><a href="ListarDepartamentos.aspx">Listar</a></li>
    <li class="breadcrumb-item active">Editar</li>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <div class="row">
            <div class="col-md-12">
                <div class="card">
                    <div class="card-header bg-warning text-dark">
                        <h4 class="mb-0">
                            <i class="fas fa-edit"></i> Editar Departamento
                        </h4>
                    </div>
                    <div class="card-body">
                        <div id="divMensaje" runat="server" visible="false" class="alert">
                            <asp:Label ID="lblMensaje" runat="server" Text=""></asp:Label>
                        </div>
                        
                        <asp:HiddenField ID="hdnDepartamentoId" runat="server" />
                        
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <label for="txtNombre" class="form-label">Nombre del Departamento *</label>
                                <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" 
                                    MaxLength="100"></asp:TextBox>
                                <div class="invalid-feedback" id="errorNombre"></div>
                                <small class="form-text text-muted">Debe ser único en el sistema</small>
                            </div>
                            
                            <div class="col-md-6 mb-3">
                                <label for="txtDescripcion" class="form-label">Descripción</label>
                                <asp:TextBox ID="txtDescripcion" runat="server" CssClass="form-control" 
                                    MaxLength="500" TextMode="MultiLine" Rows="3"></asp:TextBox>
                                <div class="invalid-feedback" id="errorDescripcion"></div>
                            </div>
                            
                            <div class="col-md-6 mb-3">
                                <label for="ddlEstado" class="form-label">Estado</label>
                                <asp:DropDownList ID="ddlEstado" runat="server" CssClass="form-select">
                                    <asp:ListItem Value="true" Text="Activo"></asp:ListItem>
                                    <asp:ListItem Value="false" Text="Inactivo"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        
                        <div class="row mt-3">
                            <div class="col-md-12">
                                <div class="card bg-light info-card">
                                    <div class="card-body">
                                        <h6 class="card-title">
                                            <i class="fas fa-info-circle"></i> Información del Departamento
                                        </h6>
                                        <div class="row">
                                            <div class="col-md-4">
                                                <strong>Fecha de Creación:</strong>
                                                <span id="lblFechaCreacion" runat="server" class="text-muted">--</span>
                                            </div>
                                            <div class="col-md-4">
                                                <strong>Última Actualización:</strong>
                                                <span id="lblFechaActualizacion" runat="server" class="text-muted">--</span>
                                            </div>
                                            <div class="col-md-4">
                                                <strong>Empleados Asignados:</strong>
                                                <span id="lblEmpleadosAsignados" runat="server" class="text-muted">--</span>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        
                        <div class="row mt-4">
                            <div class="col-md-12 text-end">
                                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" 
                                    CssClass="btn btn-secondary me-2" OnClick="btnCancelar_Click" />
                                <asp:Button ID="btnDesactivar" runat="server" Text="Desactivar" 
                                    CssClass="btn btn-danger me-2" OnClick="btnDesactivar_Click" 
                                    OnClientClick="return confirm('¿Está seguro de desactivar este departamento?\\n\\nNOTA: Los empleados asignados también serán desactivados.');" />
                                <asp:Button ID="btnGuardar" runat="server" Text="Guardar Cambios" 
                                    CssClass="btn btn-warning" OnClick="btnGuardar_Click" 
                                    OnClientClick="return validarFormularioEditar();" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="Scripts" runat="server">
    <script src="../../Scripts/departamentos.js"></script>
    <script>
        document.addEventListener('DOMContentLoaded', function() {
            document.getElementById('<%= txtNombre.ClientID %>').addEventListener('blur', validarNombreDepartamentoEditar);
        });

        function validarFormularioEditar() {
            const txtNombre = document.getElementById('<%= txtNombre.ClientID %>');
            
            if (!txtNombre.value.trim()) {
                mostrarErrorCampo('errorNombre', 'El nombre del departamento es requerido');
                txtNombre.focus();
                return false;
            }
            
            return true;
        }

        function validarNombreDepartamentoEditar() {
            const txtNombre = document.getElementById('<%= txtNombre.ClientID %>');
            const hdnDepartamentoId = document.getElementById('<%= hdnDepartamentoId.ClientID %>');
            
            if (!txtNombre || !txtNombre.value.trim()) return;
            
            const nombre = txtNombre.value.trim();
            const departamentoId = hdnDepartamentoId ? hdnDepartamentoId.value : null;
            
            $.ajax({
                type: "POST",
                url: "/WebServices/DepartamentoService.asmx/ValidarNombreDepartamento",
                data: JSON.stringify({
                    nombre: nombre,
                    excludeDepartamentoId: departamentoId
                }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function(response) {
                    const esValido = response.d;
                    
                    if (!esValido) {
                        mostrarErrorCampo('errorNombre', 'Ya existe un departamento con este nombre');
                        txtNombre.focus();
                    } else {
                        limpiarErrorCampo('errorNombre');
                    }
                },
                error: function() {
                    console.error("Error al validar nombre de departamento");
                }
            });
        }
    </script>
</asp:Content>
