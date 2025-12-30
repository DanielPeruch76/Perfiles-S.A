<%@ Page Title="Lista de Departamentos" Language="C#" MasterPageFile="~/Site.Master" 
    AutoEventWireup="true" CodeBehind="ListarDepartamentos.aspx.cs" 
    Inherits="PERFILES_SA.Pages.Departamentos.ListarDepartamentos" %>

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
        .badge-estado {
            font-size: 0.85em;
            padding: 0.35em 0.65em;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item">Departamentos</li>
    <li class="breadcrumb-item active">Listar</li>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid mt-4">
        <div class="row mb-4">
            <div class="col-md-12">
                <div class="card">
                    <div class="card-header bg-primary text-white">
                        <div class="d-flex justify-content-between align-items-center">
                            <h4 class="mb-0">
                                <i class="fas fa-sitemap"></i> Lista de Departamentos
                            </h4>
                            <asp:HyperLink ID="lnkRegistrar" runat="server" 
                                NavigateUrl="~/Pages/Departamentos/Registrar.aspx" 
                                CssClass="btn btn-light btn-sm">
                                <i class="fas fa-plus"></i> Nuevo Departamento
                            </asp:HyperLink>
                        </div>
                    </div>
                    <div class="card-body">
                        <div class="row mb-4">
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
                            <div class="col-md-8">
                                <label for="txtBuscar" class="form-label">Buscar (Nombre/Descripción)</label>
                                <div class="input-group">
                                    <asp:TextBox ID="txtBuscar" runat="server" CssClass="form-control" 
                                        placeholder="Nombre o descripción..."></asp:TextBox>
                                    <asp:Button ID="btnBuscar" runat="server" Text="Buscar" 
                                        CssClass="btn btn-outline-primary" OnClick="btnBuscar_Click" />
                                </div>
                            </div>
                        </div>
                        
                        <div class="table-responsive">
                            <asp:GridView ID="gvDepartamentos" runat="server" 
                                CssClass="table table-striped table-hover table-bordered"
                                AutoGenerateColumns="false" 
                                AllowPaging="true" PageSize="10"
                                OnPageIndexChanging="gvDepartamentos_PageIndexChanging"
                                OnRowCommand="gvDepartamentos_RowCommand"
                                EmptyDataText="No hay departamentos registrados"
                                DataKeyNames="DepartamentoId">
                                
                                <Columns>
                                    <asp:BoundField DataField="DepartamentoId" HeaderText="ID" 
                                        HeaderStyle-CssClass="bg-light" ItemStyle-Width="70px" />
                                    
                                    <asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                                    
                                    <asp:BoundField DataField="Descripcion" HeaderText="Descripción" />
                                    
                                    <asp:TemplateField HeaderText="Estado">
                                        <ItemTemplate>
                                            <span class='badge badge-estado <%# Convert.ToBoolean(Eval("Activo")) ? "bg-success" : "bg-danger" %>'>
                                                <%# Convert.ToBoolean(Eval("Activo")) ? "Activo" : "Inactivo" %>
                                            </span>
                                        </ItemTemplate>
                                        <ItemStyle Width="100px" />
                                    </asp:TemplateField>
                                    
                                    <asp:TemplateField HeaderText="Empleados Asignados">
                                        <ItemTemplate>
                                            <span class="badge bg-info">
                                                <%# ((PERFILES_SA.Pages.Departamentos.ListarDepartamentos)Page).ContarEmpleadosPorDepartamento(Convert.ToInt32(Eval("DepartamentoId"))) %>
                                            </span>
                                        </ItemTemplate>
                                        <ItemStyle Width="120px" />
                                    </asp:TemplateField>
                                    
                                    <asp:BoundField DataField="FechaCreacion" HeaderText="Fecha Creación" 
                                        DataFormatString="{0:dd/MM/yyyy HH:mm}" ItemStyle-Width="150px" />
                                    
                                    <asp:BoundField DataField="FechaActualizacion" HeaderText="Última Actualización" 
                                        DataFormatString="{0:dd/MM/yyyy HH:mm}" ItemStyle-Width="150px" />
                                    
                                    <asp:TemplateField HeaderText="Acciones" ItemStyle-Width="180px">
                                        <ItemTemplate>
                                            <div class="btn-group btn-group-sm action-buttons">
                                                <asp:HyperLink ID="lnkEditar" runat="server" 
                                                    CssClass="btn btn-warning btn-sm"
                                                    NavigateUrl='<%# "Editar.aspx?id=" + Eval("DepartamentoId") %>'
                                                    ToolTip="Editar">
                                                    <i class="fas fa-edit"></i>
                                                </asp:HyperLink>
                                                
                                                <asp:LinkButton ID="lnkCambiarEstado" runat="server" 
                                                    CssClass='btn btn-sm <%# Convert.ToBoolean(Eval("Activo")) ? "btn-danger" : "btn-success" %>'
                                                    CommandName="CambiarEstado"
                                                    CommandArgument='<%# Eval("DepartamentoId") %>'
                                                    ToolTip='<%# Convert.ToBoolean(Eval("Activo")) ? "Desactivar" : "Activar" %>'
                                                    OnClientClick='<%# "return confirm(\"¿Está seguro de " + (Convert.ToBoolean(Eval("Activo")) ? "desactivar" : "activar") + " este departamento? " + (Convert.ToBoolean(Eval("Activo")) ? "\\n\\nNOTA: Los empleados asignados también serán desactivados." : "") + "\");" %>'>
                                                    <i class='fas <%# Convert.ToBoolean(Eval("Activo")) ? "fa-toggle-off" : "fa-toggle-on" %>'></i>
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
                                                        <h3 id="lblTotalDepartamentos" runat="server">0</h3>
                                                        <small>Total Departamentos</small>
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
                                                <div class="card stats-card text-dark bg-info">
                                                    <div class="card-body">
                                                        <h3 id="lblTotalEmpleados" runat="server">0</h3>
                                                        <small>Empleados Asignados</small>
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
            if ($.fn.DataTable.isDataTable('#<%= gvDepartamentos.ClientID %>')) {
                $('#<%= gvDepartamentos.ClientID %>').DataTable().destroy();
            }
            
            $('#<%= gvDepartamentos.ClientID %>').DataTable({
                "pageLength": 10,
                "language": {
                    "url": "//cdn.datatables.net/plug-ins/1.10.25/i18n/Spanish.json"
                },
                "dom": '<"top"f>rt<"bottom"lip><"clear">',
                "responsive": true,
                "order": [[1, 'asc']], 
                "columnDefs": [
                    { "orderable": false, "targets": [6] } 
                ]
            });
            
            $('#<%= txtBuscar.ClientID %>').on('keyup', function() {
                const table = $('#<%= gvDepartamentos.ClientID %>').DataTable();
                table.search(this.value).draw();
            });
        });
    </script>
</asp:Content>