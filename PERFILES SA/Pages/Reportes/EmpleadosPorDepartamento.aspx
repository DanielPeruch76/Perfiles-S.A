<%@ Page Title="Reporte de Empleados" Language="C#" MasterPageFile="~/Site.Master" 
    AutoEventWireup="true" CodeBehind="EmpleadosPorDepartamento.aspx.cs" 
    Inherits="PERFILES_SA.Pages.Reportes.EmpleadosPorDepartamento" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .card {
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }
        .stats-card {
            transition: transform 0.3s ease;
        }
        .stats-card:hover {
            transform: translateY(-3px);
        }
        .table th {
            white-space: nowrap;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Breadcrumb" runat="server">
    <li class="breadcrumb-item">Reportes</li>
    <li class="breadcrumb-item active">Empleados por Departamento</li>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid mt-4">
        <div class="row">
            <div class="col-md-12">
                <div class="card">
                    <div class="card-header bg-primary text-white">
                        <h4 class="mb-0">
                            <i class="fas fa-chart-bar"></i> Reporte de Empleados por Departamento
                        </h4>
                    </div>
                    <div class="card-body">
                        <div class="row mb-4">
                            <div class="col-md-4">
                                <label for="ddlDepartamentoReporte" class="form-label">Departamento</label>
                                <asp:DropDownList ID="ddlDepartamentoReporte" runat="server" 
                                    CssClass="form-select">
                                    <asp:ListItem Value="" Text="Todos los departamentos"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            
                            <div class="col-md-3">
                                <label for="txtFechaInicio" class="form-label">Fecha Ingreso Desde</label>
                                <asp:TextBox ID="txtFechaInicio" runat="server" 
                                    CssClass="form-control" TextMode="Date"></asp:TextBox>
                            </div>
                            
                            <div class="col-md-3">
                                <label for="txtFechaFin" class="form-label">Fecha Ingreso Hasta</label>
                                <asp:TextBox ID="txtFechaFin" runat="server" 
                                    CssClass="form-control" TextMode="Date"></asp:TextBox>
                            </div>
                            
                            <div class="col-md-2 d-flex align-items-end">
                                <asp:Button ID="btnGenerarReporte" runat="server" Text="Generar Reporte" 
                                    CssClass="btn btn-primary w-100" OnClick="btnGenerarReporte_Click" />
                            </div>
                        </div>
                        
                        <div class="row mb-4">
                            <div class="col-md-12">
                                <div class="btn-group">
                                    <asp:Button ID="btnExportarPDF" runat="server" Text="Exportar a PDF" 
                                        CssClass="btn btn-danger me-2" OnClick="btnExportarPDF_Click" />
                                    <asp:Button ID="btnExportarExcel" runat="server" Text="Exportar a Excel" 
                                        CssClass="btn btn-success me-2" OnClick="btnExportarExcel_Click" />
                                </div>
                            </div>
                        </div>
                        
                        <div class="row">
                            <div class="col-md-12">
                                <div class="card">
                                    <div class="card-header bg-light">
                                        <h5 class="mb-0">Resultados del Reporte</h5>
                                    </div>
                                    <div class="card-body">
                                        <div class="row mb-4" id="divEstadisticas" runat="server" visible="false">
                                            <div class="col-md-12">
                                                <div class="row text-center">
                                                    <div class="col-md-3">
                                                        <div class="card stats-card bg-info text-white">
                                                            <div class="card-body">
                                                                <h3 id="lblTotalReporte" runat="server">0</h3>
                                                                <small>Total Empleados</small>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-3">
                                                        <div class="card stats-card bg-success text-white">
                                                            <div class="card-body">
                                                                <h3 id="lblEdadPromedio" runat="server">0</h3>
                                                                <small>Edad Promedio</small>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-3">
                                                        <div class="card stats-card bg-warning text-dark">
                                                            <div class="card-body">
                                                                <h3 id="lblSalarioPromedio" runat="server">0</h3>
                                                                <small>Salario Promedio</small>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-3">
                                                        <div class="card stats-card bg-secondary text-white">
                                                            <div class="card-body">
                                                                <h3 id="lblTiempoPromedio" runat="server">0</h3>
                                                                <small>Años Laborando</small>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        
                                        <div class="table-responsive">
                                            <asp:GridView ID="gvReporte" runat="server" 
                                                CssClass="table table-striped table-bordered"
                                                AutoGenerateColumns="false"
                                                EmptyDataText="No hay datos para mostrar"
                                                ShowFooter="true"
                                                OnRowDataBound="gvReporte_RowDataBound">
                                                
                                                <Columns>
                                                    <asp:BoundField DataField="Nombres" HeaderText="Empleado" />
                                                    
                                                    <asp:BoundField DataField="DPI" HeaderText="DPI" />
                                                    
                                                    <asp:TemplateField HeaderText="Edad">
                                                        <ItemTemplate>
                                                            <span class="badge bg-info">
                                                                <%# Eval("Edad") %>
                                                            </span>
                                                        </ItemTemplate>
                                                        <FooterTemplate>
                                                            <strong>Promedio:</strong>
                                                        </FooterTemplate>
                                                    </asp:TemplateField>
                                                    
                                                    <asp:BoundField DataField="Sexo" HeaderText="Sexo" />
                                                    
                                                    <asp:BoundField DataField="NombreDepartamento" HeaderText="Departamento" />
                                                    
                                                    <asp:BoundField DataField="FechaIngreso" HeaderText="Fecha Ingreso" 
                                                        DataFormatString="{0:dd/MM/yyyy}" />
                                                    
                                                    <asp:TemplateField HeaderText="Tiempo Laborando">
                                                        <ItemTemplate>
                                                            <span class="badge bg-success">
                                                                <%# ((PERFILES_SA.Pages.Reportes.EmpleadosPorDepartamento)Page).CalcularTiempoLaborando(Convert.ToDateTime(Eval("FechaIngreso"))) %>
                                                            </span>
                                                        </ItemTemplate>
                                                        <FooterTemplate>
                                                            <asp:Label ID="lblPromedioTiempo" runat="server"></asp:Label>
                                                        </FooterTemplate>
                                                    </asp:TemplateField>
                                                    
                                                    <asp:TemplateField HeaderText="Estado">
                                                        <ItemTemplate>
                                                            <span class='badge <%# Convert.ToBoolean(Eval("Activo")) ? "bg-success" : "bg-danger" %>'>
                                                                <%# Convert.ToBoolean(Eval("Activo")) ? "Activo" : "Inactivo" %>
                                                            </span>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                                
                                                <HeaderStyle CssClass="table-dark" />
                                                <FooterStyle CssClass="table-warning" Font-Bold="true" />
                                            </asp:GridView>
                                        </div>
                                        
                                        <div class="row mt-4" id="divGrafico" runat="server" visible="false">
                                            <div class="col-md-12">
                                                <div class="card">
                                                    <div class="card-header">
                                                        <h5 class="mb-0">Distribución por Departamento</h5>
                                                    </div>
                                                    <div class="card-body">
                                                        <canvas id="chartDepartamentos" width="400" height="200"></canvas>
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
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <script>
        document.addEventListener('DOMContentLoaded', function() {

            if ($.fn.DataTable) {
                $('#<%= gvReporte.ClientID %>').DataTable({
                    "pageLength": 10,
                    "language": {
                        "url": "//cdn.datatables.net/plug-ins/1.10.25/i18n/Spanish.json"
                    },
                    "dom": '<"top"f>rt<"bottom"lip><"clear">',
                    "responsive": true
                });
            }
        });
        
        function actualizarGrafico(labels, data) {
            var ctx = document.getElementById('chartDepartamentos').getContext('2d');
            if (window.myChart) {
                window.myChart.destroy();
            }
            
            window.myChart = new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: labels,
                    datasets: [{
                        label: 'Cantidad de Empleados',
                        data: data,
                        backgroundColor: 'rgba(54, 162, 235, 0.7)',
                        borderColor: 'rgba(54, 162, 235, 1)',
                        borderWidth: 1
                    }]
                },
                options: {
                    responsive: true,
                    plugins: {
                        legend: {
                            position: 'top',
                        },
                        title: {
                            display: true,
                            text: 'Distribución de Empleados por Departamento'
                        }
                    },
                    scales: {
                        y: {
                            beginAtZero: true
                        }
                    }
                }
            });
        }
    </script>
</asp:Content>