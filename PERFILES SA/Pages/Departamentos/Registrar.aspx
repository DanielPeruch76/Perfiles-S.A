<%@ Page Title="Registrar Departamento" Language="C#" MasterPageFile="~/Site.Master" 
    AutoEventWireup="true" CodeBehind="Registrar.aspx.cs" 
    Inherits="PERFILES_SA.Pages.Departamentos.Registrar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .form-container {
            max-width: 800px;
            margin: 0 auto;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item">Departamentos</li>
    <li class="breadcrumb-item"><a href="ListarDepartamentos.aspx">Listar</a></li>
    <li class="breadcrumb-item active">Registrar</li>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <div class="row">
            <div class="col-md-12">
                <div class="card">
                    <div class="card-header bg-primary text-white">
                        <h4 class="mb-0">
                            <i class="fas fa-plus-circle"></i> Registrar Nuevo Departamento
                        </h4>
                    </div>
                    <div class="card-body">
                        <div id="divMensaje" runat="server" visible="false" class="alert">
                            <asp:Label ID="lblMensaje" runat="server" Text=""></asp:Label>
                        </div>
                        
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <label for="txtNombre" class="form-label">Nombre del Departamento *</label>
                                <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" 
                                    MaxLength="100" placeholder="Ej: Recursos Humanos"></asp:TextBox>
                                <div class="invalid-feedback" id="errorNombre"></div>
                                <small class="form-text text-muted">Debe ser único en el sistema</small>
                            </div>
                            
                            <div class="col-md-6 mb-3">
                                <label for="txtDescripcion" class="form-label">Descripción</label>
                                <asp:TextBox ID="txtDescripcion" runat="server" CssClass="form-control" 
                                    MaxLength="500" TextMode="MultiLine" Rows="3" 
                                    placeholder="Descripción del departamento..."></asp:TextBox>
                                <div class="invalid-feedback" id="errorDescripcion"></div>
                            </div>
                        </div>
                        
                        <div class="row mt-4">
                            <div class="col-md-12 text-end">
                                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" 
                                    CssClass="btn btn-secondary me-2" OnClick="btnCancelar_Click" />
                                <asp:Button ID="btnGuardar" runat="server" Text="Guardar Departamento" 
                                    CssClass="btn btn-primary" OnClick="btnGuardar_Click" 
                                    OnClientClick="return validarFormularioDepartamento();" />
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
            document.getElementById('<%= txtNombre.ClientID %>').addEventListener('blur', validarNombreDepartamento);
        });

        function validarFormularioDepartamento() {
            const txtNombre = document.getElementById('<%= txtNombre.ClientID %>');
            
            if (!txtNombre.value.trim()) {
                mostrarErrorCampo('errorNombre', 'El nombre del departamento es requerido');
                txtNombre.focus();
                return false;
            }
            
            return true;
        }

        function validarNombreDepartamento() {
            const txtNombre = document.getElementById('<%= txtNombre.ClientID %>');
            
            if (!txtNombre || !txtNombre.value.trim()) return;
            
            const nombre = txtNombre.value.trim();
            
            $.ajax({
                type: "POST",
                url: "/WebServices/DepartamentoService.asmx/ValidarNombreDepartamento",
                data: JSON.stringify({
                    nombre: nombre,
                    excludeDepartamentoId: null
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
