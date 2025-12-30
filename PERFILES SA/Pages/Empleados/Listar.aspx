<%@ Page Title="Lista de Empleados" Language="C#" MasterPageFile="~/Site.Master" 
    AutoEventWireup="true" CodeBehind="Listar.aspx.cs" 
    Inherits="PERFILES_SA.Pages.Empleados.Listar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .stats-card {
            transition: transform 0.3s ease;
            border: none;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }
        .stats-card:hover {
            transform: translateY(-5px);
        }
        .action-buttons .btn {
            margin: 0 2px;
        }
        .table th {
            white-space: nowrap;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item">Empleados</li>
    <li class="breadcrumb-item active">Listar</li>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid mt-4">
        <div class="row mb-4">
            <div class="col-md-12">
                <div class="card">
                    <div class="card-header bg-info text-white">
                        <div class="d-flex justify-content-between align-items-center">
                            <h4 class="mb-0">
                                <i class="fas fa-users"></i> Lista de Empleados
                            </h4>
                            <asp:HyperLink ID="lnkRegistrar" runat="server" 
                                NavigateUrl="~/Pages/Empleados/Registro.aspx" 
                                CssClass="btn btn-light btn-sm">
                                <i class="fas fa-plus"></i> Nuevo Empleado
                            </asp:HyperLink>
                        </div>
                    </div>
                    <div class="card-body">
                        <div class="row mb-4">
                            <div class="col-md-4">
                                <label for="ddlFiltroDepartamento" class="form-label">Filtrar por Departamento</label>
                                <asp:DropDownList ID="ddlFiltroDepartamento" runat="server" 
                                    CssClass="form-select" AutoPostBack="true" 
                                    OnSelectedIndexChanged="ddlFiltroDepartamento_SelectedIndexChanged">
                                    <asp:ListItem Value="" Text="Todos los departamentos"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-4">
                                <label for="ddlFiltroEstado" class="form-label">Filtrar por Estado</label>
                                <asp:DropDownList ID="ddlFiltroEstado" runat="server" 
                                    CssClass="form-select" AutoPostBack="true"
                                    OnSelectedIndexChanged="ddlFiltroEstado_SelectedIndexChanged">
                                    <asp:ListItem Value="" Text="Todos"></asp:ListItem>
                                    <asp:ListItem Value="true" Text="Activos"></asp:ListItem>
                                    <asp:ListItem Value="false" Text="Inactivos"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-4">
                                <label for="txtBuscar" class="form-label">Buscar (DPI/Nombre)</label>
                                <div class="input-group">
                                    <asp:TextBox ID="txtBuscar" runat="server" CssClass="form-control" 
                                        placeholder="DPI o nombre..."></asp:TextBox>
                                    <asp:Button ID="btnBuscar" runat="server" Text="Buscar" 
                                        CssClass="btn btn-outline-primary" OnClick="btnBuscar_Click" />
                                </div>
                            </div>
                        </div>
                        
                        <div class="table-responsive">
                            <asp:GridView ID="gvEmpleados" runat="server" 
                                CssClass="table table-striped table-hover table-bordered"
                                AutoGenerateColumns="false" 
                                AllowPaging="true" PageSize="10"
                                OnPageIndexChanging="gvEmpleados_PageIndexChanging"
                                OnRowCommand="gvEmpleados_RowCommand"
                                EmptyDataText="No hay empleados registrados"
                                DataKeyNames="EmpleadoId">
                                
                                <Columns>
                                    <asp:BoundField DataField="EmpleadoId" HeaderText="ID" 
                                        HeaderStyle-CssClass="bg-light" ItemStyle-Width="70px" />
                                    
                                    <asp:BoundField DataField="DPI" HeaderText="DPI" />
                                    
                                    <asp:BoundField DataField="Nombres" HeaderText="Nombres Completos" />
                                    
                                    <asp:TemplateField HeaderText="Edad">
                                        <ItemTemplate>
                                            <span class="badge bg-info">
                                                <%# ((PERFILES_SA.Pages.Empleados.Listar)Page).CalcularEdad(Convert.ToDateTime(Eval("FechaNacimiento"))) %>
                                            </span>
                                        </ItemTemplate>
                                        <ItemStyle Width="80px" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="FechaNacimiento" HeaderText="Fecha Nacimiento" 
                                    DataFormatString="{0:dd/MM/yyyy}" ItemStyle-Width="120px" />
                                    
                                    <asp:BoundField DataField="Sexo" HeaderText="Sexo" 
                                        ItemStyle-Width="80px" />
                                    
                                    <asp:BoundField DataField="NombreDepartamento" HeaderText="Departamento" />
                                    
                                    <asp:TemplateField HeaderText="Estado">
                                        <ItemTemplate>
                                            <span class='badge <%# Convert.ToBoolean(Eval("Activo")) ? "bg-success" : "bg-danger" %>'>
                                                <%# Convert.ToBoolean(Eval("Activo")) ? "Activo" : "Inactivo" %>
                                            </span>
                                        </ItemTemplate>
                                        <ItemStyle Width="100px" />
                                    </asp:TemplateField>
                                    
                                    <asp:BoundField DataField="FechaIngreso" HeaderText="Fecha Ingreso" 
                                        DataFormatString="{0:dd/MM/yyyy}" ItemStyle-Width="120px" />
                                    
                                    <asp:TemplateField HeaderText="Acciones" ItemStyle-Width="180px">
                                        <ItemTemplate>
                                            <div class="btn-group btn-group-sm action-buttons">
                                                <asp:HyperLink ID="lnkEditar" runat="server" 
                                                    CssClass="btn btn-warning btn-sm"
                                                    NavigateUrl='<%# "Editar.aspx?id=" + Eval("EmpleadoId") %>'
                                                    ToolTip="Editar">
                                                    <i class="fas fa-edit"></i>
                                                </asp:HyperLink>
                                                
                                                <asp:LinkButton ID="lnkDesactivar" runat="server" 
                                                    CssClass='btn btn-sm <%# Convert.ToBoolean(Eval("Activo")) ? "btn-danger" : "btn-secondary" %>'
                                                    CommandName="CambiarEstado"
                                                    CommandArgument='<%# Eval("EmpleadoId") %>'
                                                    ToolTip='<%# Convert.ToBoolean(Eval("Activo")) ? "Desactivar" : "Activar" %>'
                                                    OnClientClick='<%# "return confirm(\"¿Está seguro de " + (Convert.ToBoolean(Eval("Activo")) ? "desactivar" : "activar") + " este empleado?\");" %>'>
                                                    <i class='fas <%# Convert.ToBoolean(Eval("Activo")) ? "fa-user-times" : "fa-user-check" %>'></i>
                                                </asp:LinkButton>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                
                                <PagerStyle CssClass="pagination" />
                                <PagerSettings Mode="NumericFirstLast" 
                                    PageButtonCount="5" 
                                    Position="Bottom" />
                                
                                <HeaderStyle CssClass="table-dark" />
                                <RowStyle CssClass="align-middle" />
                                <AlternatingRowStyle CssClass="table-light" />
                            </asp:GridView>
                        </div>
                        
                        <div class="row mt-4">
                            <div class="col-md-12">
                                <div class="card bg-light">
                                    <div class="card-body">
                                        <div class="row text-center">
                                            <div class="col-md-3">
                                                <div class="card stats-card text-white bg-primary">
                                                    <div class="card-body">
                                                        <h3 id="lblTotalEmpleados" runat="server">0</h3>
                                                        <small>Total Empleados</small>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-md-3">
                                                <div class="card stats-card text-white bg-success">
                                                    <div class="card-body">
                                                        <h3 id="lblActivos" runat="server">0</h3>
                                                        <small>Activos</small>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-md-3">
                                                <div class="card stats-card text-white bg-danger">
                                                    <div class="card-body">
                                                        <h3 id="lblInactivos" runat="server">0</h3>
                                                        <small>Inactivos</small>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-md-3">
                                                <div class="card stats-card text-dark bg-warning">
                                                    <div class="card-body">
                                                        <h3 id="lblPromedioEdad" runat="server">0</h3>
                                                        <small>Edad Promedio</small>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="Scripts" runat="server">
    <script>
        document.addEventListener('DOMContentLoaded', function() {
            if ($.fn.DataTable.isDataTable('#<%= gvEmpleados.ClientID %>')) {
                $('#<%= gvEmpleados.ClientID %>').DataTable().destroy();
            }
            
            $('#<%= gvEmpleados.ClientID %>').DataTable({
                "pageLength": 10,
                "language": {
                    "url": "//cdn.datatables.net/plug-ins/1.10.25/i18n/Spanish.json"
                },
                "dom": '<"top"f>rt<"bottom"lip><"clear">',
                "responsive": true,
                "order": [[2, 'asc']], 
                "columnDefs": [
                    { "orderable": false, "targets": [7] } 
                ]
            });
            
            $('#<%= txtBuscar.ClientID %>').on('keyup', function() {
                const table = $('#<%= gvEmpleados.ClientID %>').DataTable();
                table.search(this.value).draw();
            });
            
            actualizarEstadisticasUI();
        });
        
        function actualizarEstadisticasUI() {
            console.log("Estadísticas actualizadas");
        }
    </script>
</asp:Content>