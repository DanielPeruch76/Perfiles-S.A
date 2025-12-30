<%@ Page Title="Editar Empleado" Language="C#" MasterPageFile="~/Site.Master" 
    AutoEventWireup="true" CodeBehind="Editar.aspx.cs" 
    Inherits="PERFILES_SA.Pages.Empleados.Editar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .info-card {
            background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
            border-left: 4px solid #ffc107;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item">Empleados</li>
    <li class="breadcrumb-item"><a href="Listar.aspx">Listar</a></li>
    <li class="breadcrumb-item active">Editar</li>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <div class="row">
            <div class="col-md-12">
                <div class="card">
                    <div class="card-header bg-warning text-dark">
                        <h4 class="mb-0">
                            <i class="fas fa-user-edit"></i> Editar Empleado
                        </h4>
                    </div>
                    <div class="card-body">
                        <div id="divMensaje" runat="server" visible="false" class="alert">
                            <asp:Label ID="lblMensaje" runat="server" Text=""></asp:Label>
                        </div>
                        
                        <asp:HiddenField ID="hdnEmpleadoId" runat="server" />
                        
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <label for="txtDPI" class="form-label">DPI *</label>
                                <asp:TextBox ID="txtDPI" runat="server" CssClass="form-control" 
                                    MaxLength="20"></asp:TextBox>
                                <div class="invalid-feedback" id="errorDPI"></div>
                                <small class="form-text text-muted">Debe ser único en el sistema</small>
                            </div>
                            
                            <div class="col-md-6 mb-3">
                                <label for="txtNombres" class="form-label">Nombres Completos *</label>
                                <asp:TextBox ID="txtNombres" runat="server" CssClass="form-control" 
                                    MaxLength="100"></asp:TextBox>
                                <div class="invalid-feedback" id="errorNombres"></div>
                            </div>
                            
                            <div class="col-md-4 mb-3">
                                <label for="txtFechaNacimiento" class="form-label">Fecha de Nacimiento *</label>
                                <asp:TextBox ID="txtFechaNacimiento" runat="server" CssClass="form-control datepicker" 
                                    TextMode="Date"></asp:TextBox>
                                <div class="invalid-feedback" id="errorFechaNacimiento"></div>
                                <small class="form-text text-muted">
                                    Edad: <span id="lblEdad" class="badge bg-info">--</span> años
                                </small>
                            </div>
                           
                            <div class="col-md-2 mb-3">
                                <label for="ddlSexo" class="form-label">Sexo *</label>
                                <asp:DropDownList ID="ddlSexo" runat="server" CssClass="form-select">
                                    <asp:ListItem Value="" Text="Seleccionar"></asp:ListItem>
                                    <asp:ListItem Value="M" Text="Masculino"></asp:ListItem>
                                    <asp:ListItem Value="F" Text="Femenino"></asp:ListItem>
                                </asp:DropDownList>
                                <div class="invalid-feedback" id="errorSexo"></div>
                            </div>
                            
                            <div class="col-md-4 mb-3">
                                <label for="txtFechaIngreso" class="form-label">Fecha de Ingreso *</label>
                                <asp:TextBox ID="txtFechaIngreso" runat="server" CssClass="form-control datepicker" 
                                    TextMode="Date"></asp:TextBox>
                                <div class="invalid-feedback" id="errorFechaIngreso"></div>
                                <small class="form-text text-muted">
                                    Tiempo laborando: <span id="lblTiempoLaborando" class="badge bg-success">--</span>
                                </small>
                            </div>
                            
                            <div class="col-md-2 mb-3">
                                <label for="ddlDepartamento" class="form-label">Departamento *</label>
                                <asp:DropDownList ID="ddlDepartamento" runat="server" CssClass="form-select">
                                    <asp:ListItem Value="" Text="Cargando..."></asp:ListItem>
                                </asp:DropDownList>
                                <div class="invalid-feedback" id="errorDepartamento"></div>
                            </div>
                            
                            <div class="col-md-6 mb-3">
                                <label for="txtDireccion" class="form-label">Dirección</label>
                                <asp:TextBox ID="txtDireccion" runat="server" CssClass="form-control" 
                                    MaxLength="500" TextMode="MultiLine" Rows="2"></asp:TextBox>
                            </div>
                            
                            <div class="col-md-6 mb-3">
                                <label for="txtNIT" class="form-label">NIT</label>
                                <asp:TextBox ID="txtNIT" runat="server" CssClass="form-control" 
                                    MaxLength="20"></asp:TextBox>
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
                                            <i class="fas fa-info-circle"></i> Información del Empleado
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
                                                <strong>Departamento Actual:</strong>
                                                <span id="lblDepartamentoActual" runat="server" class="text-muted">--</span>
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
                                    OnClientClick="return confirm('¿Está seguro de desactivar este empleado?');" />
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
    <script src="../../Scripts/empleados.js"></script>
    <script>
        document.addEventListener('DOMContentLoaded', function() {
            $('.datepicker').datepicker({
                format: 'yyyy-mm-dd',
                autoclose: true
            });
            
            cargarDepartamentos();
            
            document.getElementById('<%= txtFechaNacimiento.ClientID %>').addEventListener('change', calcularEdad);
            document.getElementById('<%= txtFechaIngreso.ClientID %>').addEventListener('change', calcularTiempoLaborando);
            document.getElementById('<%= txtDPI.ClientID %>').addEventListener('blur', validarDPIUnicoEditar);
        });

        function validarFormularioEditar() {
            return validarFormulario(); 

        function validarDPIUnicoEditar() {
            const txtDPI = document.getElementById('<%= txtDPI.ClientID %>');
            const hdnEmpleadoId = document.getElementById('<%= hdnEmpleadoId.ClientID %>');
            
            if (!txtDPI || !txtDPI.value.trim()) return;
            
            const dpi = txtDPI.value.trim();
            const empleadoId = hdnEmpleadoId ? hdnEmpleadoId.value : null;
            
            $.ajax({
                type: "POST",
                url: "/WebServices/EmpleadoService.asmx/ValidarDPI",
                data: JSON.stringify({
                    dpi: dpi,
                    excludeEmpleadoId: empleadoId
                }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function(response) {
                    const esValido = response.d;
                    
                    if (!esValido) {
                        mostrarErrorCampo('errorDPI', 'El DPI ya está registrado en el sistema');
                        txtDPI.focus();
                    } else {
                        limpiarErrorCampo('errorDPI');
                    }
                },
                error: function() {
                    console.error("Error al validar DPI");
                }
            });
        }
    </script>
</asp:Content>