<%@ Page Title="Registro de Empleado" Language="C#" MasterPageFile="~/Site.Master" 
    AutoEventWireup="true" CodeBehind="Registro.aspx.cs" 
    Inherits="PERFILES_SA.Pages.Empleados.Registro" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .form-container {
            max-width: 1200px;
            margin: 0 auto;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item">Empleados</li>
    <li class="breadcrumb-item active">Registro</li>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <div class="row">
            <div class="col-md-12">
                <div class="card">
                    <div class="card-header bg-primary text-white">
                        <h4 class="mb-0">
                            <i class="fas fa-user-plus"></i> Registro de Empleado
                        </h4>
                    </div>
                    <div class="card-body">
                        <div id="divMensaje" runat="server" visible="false" class="alert">
                            <asp:Label ID="lblMensaje" runat="server" Text=""></asp:Label>
                        </div>
                        
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <label for="txtDPI" class="form-label">DPI *</label>
                                <asp:TextBox ID="txtDPI" runat="server" CssClass="form-control" 
                                    MaxLength="20" placeholder="Número de DPI"></asp:TextBox>
                                <div class="invalid-feedback" id="errorDPI"></div>
                                <small class="form-text text-muted">Debe ser único en el sistema</small>
                            </div>
                            
                            <div class="col-md-6 mb-3">
                                <label for="txtNombres" class="form-label">Nombres Completos *</label>
                                <asp:TextBox ID="txtNombres" runat="server" CssClass="form-control" 
                                    MaxLength="100" placeholder="Nombres y apellidos"></asp:TextBox>
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
                                    MaxLength="500" TextMode="MultiLine" Rows="2" 
                                    placeholder="Dirección completa"></asp:TextBox>
                            </div>
                            
                            <div class="col-md-6 mb-3">
                                <label for="txtNIT" class="form-label">NIT</label>
                                <asp:TextBox ID="txtNIT" runat="server" CssClass="form-control" 
                                    MaxLength="20" placeholder="Número de NIT"></asp:TextBox>
                            </div>
                        </div>
                        
                        <div class="row mt-4">
                            <div class="col-md-12 text-end">
                                <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar" 
                                    CssClass="btn btn-secondary me-2" OnClick="btnLimpiar_Click" />
                                <asp:Button ID="btnGuardar" runat="server" Text="Guardar Empleado" 
                                    CssClass="btn btn-primary" OnClick="btnGuardar_Click" 
                                    OnClientClick="return validarFormulario();" />
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
        document.addEventListener('DOMContentLoaded', function () {
            $('.datepicker').datepicker({
                format: 'yyyy-mm-dd',
                autoclose: true,
                todayHighlight: true
            });

            cargarDepartamentos();

            document.getElementById('<%= txtFechaNacimiento.ClientID %>').addEventListener('change', calcularEdad);
            document.getElementById('<%= txtFechaIngreso.ClientID %>').addEventListener('change', calcularTiempoLaborando);

            document.getElementById('<%= txtDPI.ClientID %>').addEventListener('blur', validarDPIUnico);
        });
    </script>
</asp:Content>