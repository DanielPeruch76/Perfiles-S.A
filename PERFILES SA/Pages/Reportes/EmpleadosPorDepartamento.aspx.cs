using PERFILES_SA.Models;
using PERFILES_SA.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PERFILES_SA.Pages.Reportes
{
    public partial class EmpleadosPorDepartamento : System.Web.UI.Page
    {
        private readonly IEmpleadoService _empleadoService;
        private List<Empleado> _empleadosReporte;

        public EmpleadosPorDepartamento()
        {
            _empleadoService = new PERFILES_SA.Services.EmpleadoService();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarDepartamentos();

                txtFechaInicio.Text = DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd");
                txtFechaFin.Text = DateTime.Now.ToString("yyyy-MM-dd");
            }
        }

        private void CargarDepartamentos()
        {
            try
            {
                var departamentos = _empleadoService.ObtenerDepartamentosActivos();

                ddlDepartamentoReporte.Items.Clear();
                ddlDepartamentoReporte.Items.Add(new ListItem("Todos los departamentos", ""));

                foreach (var depto in departamentos)
                {
                    ddlDepartamentoReporte.Items.Add(new ListItem(depto.Nombre, depto.DepartamentoId.ToString()));
                }
            }
            catch (Exception ex)
            {
                MostrarError($"Error al cargar departamentos: {ex.Message}");
            }
        }

        protected void btnGenerarReporte_Click(object sender, EventArgs e)
        {
            try
            {
                GenerarReporte();
                CalcularEstadisticas();
                divEstadisticas.Visible = true;

                if (_empleadosReporte != null && _empleadosReporte.Count > 0)
                {
                    divGrafico.Visible = true;
                    GenerarGrafico();
                }
            }
            catch (Exception ex)
            {
                MostrarError($"Error al generar reporte: {ex.Message}");
            }
        }

        private void GenerarReporte()
        {
            var todosEmpleados = _empleadoService.ObtenerTodosEmpleados();

            if (todosEmpleados == null)
            {
                _empleadosReporte = new List<Empleado>();
                gvReporte.DataSource = _empleadosReporte;
                gvReporte.DataBind();
                return;
            }

            var empleadosFiltrados = todosEmpleados.AsQueryable();

            if (!string.IsNullOrEmpty(ddlDepartamentoReporte.SelectedValue))
            {
                int departamentoId = int.Parse(ddlDepartamentoReporte.SelectedValue);
                empleadosFiltrados = empleadosFiltrados.Where(e => e.DepartamentoId == departamentoId);
            }

            if (!string.IsNullOrEmpty(txtFechaInicio.Text))
            {
                DateTime fechaInicio = DateTime.Parse(txtFechaInicio.Text);
                empleadosFiltrados = empleadosFiltrados.Where(e => e.FechaIngreso >= fechaInicio);
            }

            if (!string.IsNullOrEmpty(txtFechaFin.Text))
            {
                DateTime fechaFin = DateTime.Parse(txtFechaFin.Text);
                empleadosFiltrados = empleadosFiltrados.Where(e => e.FechaIngreso <= fechaFin);
            }

            _empleadosReporte = empleadosFiltrados.ToList();

            gvReporte.DataSource = _empleadosReporte;
            gvReporte.DataBind();
        }

        private void CalcularEstadisticas()
        {
            if (_empleadosReporte == null || _empleadosReporte.Count == 0)
            {
                lblTotalReporte.InnerText = "0";
                lblEdadPromedio.InnerText = "0";
                lblSalarioPromedio.InnerText = "N/A";
                lblTiempoPromedio.InnerText = "0";
                return;
            }

            int total = _empleadosReporte.Count;
            double edadPromedio = _empleadosReporte.Average(e => e.Edad);

            double tiempoPromedio = _empleadosReporte.Average(e => {
                var hoy = DateTime.Today;
                var tiempo = hoy.Year - e.FechaIngreso.Year;
                if (e.FechaIngreso.Date > hoy.AddYears(-tiempo)) tiempo--;
                return tiempo;
            });

            lblTotalReporte.InnerText = total.ToString("N0");
            lblEdadPromedio.InnerText = Math.Round(edadPromedio, 1).ToString();
            lblTiempoPromedio.InnerText = Math.Round(tiempoPromedio, 1).ToString();
            lblSalarioPromedio.InnerText = "N/A";
        }

        public string CalcularTiempoLaborando(DateTime fechaIngreso)
        {
            var hoy = DateTime.Today;
            var anios = hoy.Year - fechaIngreso.Year;
            if (fechaIngreso.Date > hoy.AddYears(-anios)) anios--;

            var meses = hoy.Month - fechaIngreso.Month;
            if (meses < 0)
            {
                meses += 12;
                anios--;
            }

            return $"{anios} años {(meses > 0 ? meses + " meses" : "")}".Trim();
        }

        protected void gvReporte_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                if (_empleadosReporte != null && _empleadosReporte.Count > 0)
                {
                    double edadPromedio = _empleadosReporte.Average(emp => emp.Edad);

                    double tiempoPromedio = _empleadosReporte.Average(e2 => {
                        var hoy = DateTime.Today;
                        var tiempo = hoy.Year - e2.FechaIngreso.Year;
                        if (e2.FechaIngreso.Date > hoy.AddYears(-tiempo)) tiempo--;
                        return tiempo;
                    });

                    e.Row.Cells[2].Text = Math.Round(edadPromedio, 1).ToString();

                    var lblPromedioTiempo = e.Row.FindControl("lblPromedioTiempo") as Label;
                    if (lblPromedioTiempo != null)
                    {
                        lblPromedioTiempo.Text = Math.Round(tiempoPromedio, 1).ToString() + " años";
                    }
                }
            }
        }

        private void GenerarGrafico()
        {
            if (_empleadosReporte == null || _empleadosReporte.Count == 0)
                return;

            try
            {
                var datosPorDepartamento = _empleadosReporte
                    .Where(e => !string.IsNullOrEmpty(e.NombreDepartamento))
                    .GroupBy(e => e.NombreDepartamento)
                    .Select(g => new
                    {
                        Departamento = g.Key,
                        Cantidad = g.Count()
                    })
                    .OrderByDescending(d => d.Cantidad)
                    .ToList();

                if (datosPorDepartamento.Count == 0)
                    return;

                var labels = datosPorDepartamento.Select(d => d.Departamento).ToList();
                var data = datosPorDepartamento.Select(d => d.Cantidad).ToList();

                string script = $@"
                    var labels = {Newtonsoft.Json.JsonConvert.SerializeObject(labels)};
                    var data = {Newtonsoft.Json.JsonConvert.SerializeObject(data)};
                    
                    if (typeof actualizarGrafico === 'function') {{
                        actualizarGrafico(labels, data);
                    }}";

                ScriptManager.RegisterStartupScript(this, GetType(), "GenerarGrafico", script, true);
            }
            catch (Exception ex)
            {
            }
        }

        protected void btnExportarPDF_Click(object sender, EventArgs e)
        {
            try
            {
                if (_empleadosReporte == null || _empleadosReporte.Count == 0)
                {
                    MostrarError("No hay datos para exportar");
                    return;
                }

                using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
                {
                    iTextSharp.text.Document document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate());
                    iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, memoryStream);

                    document.Open();

                    iTextSharp.text.Font titleFont = iTextSharp.text.FontFactory.GetFont("Arial", 18, iTextSharp.text.Font.BOLD);
                    iTextSharp.text.Paragraph title = new iTextSharp.text.Paragraph("Reporte de Empleados por Departamento", titleFont);
                    title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    title.SpacingAfter = 20;
                    document.Add(title);

                    iTextSharp.text.Font infoFont = iTextSharp.text.FontFactory.GetFont("Arial", 10);
                    iTextSharp.text.Paragraph info = new iTextSharp.text.Paragraph($"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm}", infoFont);
                    info.SpacingAfter = 10;
                    document.Add(info);

                    if (!string.IsNullOrEmpty(ddlDepartamentoReporte.SelectedItem.Text) && ddlDepartamentoReporte.SelectedValue != "")
                    {
                        document.Add(new iTextSharp.text.Paragraph($"Departamento: {ddlDepartamentoReporte.SelectedItem.Text}", infoFont));
                    }

                    document.Add(new iTextSharp.text.Paragraph($"Total de empleados: {_empleadosReporte.Count}", infoFont));
                    document.Add(iTextSharp.text.Chunk.NEWLINE);

                    iTextSharp.text.pdf.PdfPTable table = new iTextSharp.text.pdf.PdfPTable(9); // 9 columnas
                    table.WidthPercentage = 100;

                    float[] columnWidths = new float[] { 0.5f, 2f, 1.5f, 0.8f, 0.8f, 1.5f, 1.2f, 1.5f, 0.8f };
                    table.SetWidths(columnWidths);

                    iTextSharp.text.Font headerFont = iTextSharp.text.FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.WHITE);
                    iTextSharp.text.pdf.PdfPCell headerCell;

                    string[] headers = { "No.", "Empleado", "DPI", "Edad", "Sexo", "Departamento", "Fecha Ingreso", "Tiempo Laborando", "Estado" };

                    foreach (string header in headers)
                    {
                        headerCell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(header, headerFont));
                        headerCell.BackgroundColor = new iTextSharp.text.BaseColor(79, 129, 189); // Azul
                        headerCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                        headerCell.Padding = 5;
                        table.AddCell(headerCell);
                    }

                    iTextSharp.text.Font cellFont = iTextSharp.text.FontFactory.GetFont("Arial", 9);
                    int rowNumber = 1;

                    foreach (var emp in _empleadosReporte)
                    {
                        table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(rowNumber.ToString(), cellFont)));

                        table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(emp.Nombres, cellFont)));

                        table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(emp.DPI, cellFont)));

                        table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(emp.Edad.ToString(), cellFont)));

                        table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(emp.Sexo, cellFont)));

                        table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(emp.NombreDepartamento ?? "Sin asignar", cellFont)));

                        table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(emp.FechaIngreso.ToString("dd/MM/yyyy"), cellFont)));

                        table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(CalcularTiempoLaborando(emp.FechaIngreso), cellFont)));

                        iTextSharp.text.Font estadoFont = iTextSharp.text.FontFactory.GetFont("Arial", 9,
                            emp.Activo ? iTextSharp.text.Font.BOLD : iTextSharp.text.Font.NORMAL,
                            emp.Activo ? iTextSharp.text.BaseColor.GREEN : iTextSharp.text.BaseColor.RED);
                        table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(emp.Activo ? "Activo" : "Inactivo", estadoFont)));

                        rowNumber++;
                    }

                    document.Add(table);
                    document.Close();

                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", $"attachment;filename=Reporte_Empleados_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.BinaryWrite(memoryStream.ToArray());

                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception ex)
            {
                MostrarError($"Error al exportar a PDF: {ex.Message}");
            }
        }

        protected void btnExportarExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (_empleadosReporte == null || _empleadosReporte.Count == 0)
                {
                    MostrarError("No hay datos para exportar");
                    return;
                }

                Response.Clear();
                Response.Buffer = true;

                Response.AddHeader("content-disposition",
                    $"attachment;filename=Reporte_Empleados_{DateTime.Now:yyyyMMdd_HHmmss}.xls");
                Response.Charset = "";
                Response.ContentType = "application/vnd.ms-excel";

                using (System.IO.StringWriter sw = new System.IO.StringWriter())
                {
                    using (System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(sw))
                    {
                        string html = "<html>";
                        html += "<head><meta charset='UTF-8'></head>";
                        html += "<body>";
                        html += "<h2>Reporte de Empleados por Departamento</h2>";
                        html += $"<p><strong>Fecha de generación:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}</p>";

                        if (!string.IsNullOrEmpty(ddlDepartamentoReporte.SelectedItem.Text) &&
                            ddlDepartamentoReporte.SelectedValue != "")
                        {
                            html += $"<p><strong>Departamento:</strong> {ddlDepartamentoReporte.SelectedItem.Text}</p>";
                        }

                        if (!string.IsNullOrEmpty(txtFechaInicio.Text))
                        {
                            html += $"<p><strong>Fecha desde:</strong> {txtFechaInicio.Text}</p>";
                        }

                        if (!string.IsNullOrEmpty(txtFechaFin.Text))
                        {
                            html += $"<p><strong>Fecha hasta:</strong> {txtFechaFin.Text}</p>";
                        }

                        html += "<br/>";

                        html += "<table border='1' cellpadding='5' cellspacing='0'>";
                        html += "<tr style='background-color: #4CAF50; color: white; font-weight: bold;'>";
                        html += "<th>No.</th>";
                        html += "<th>Empleado</th>";
                        html += "<th>DPI</th>";
                        html += "<th>Edad</th>";
                        html += "<th>Sexo</th>";
                        html += "<th>Departamento</th>";
                        html += "<th>Fecha Ingreso</th>";
                        html += "<th>Tiempo Laborando</th>";
                        html += "<th>Estado</th>";
                        html += "</tr>";

                        int contador = 1;
                        foreach (var emp in _empleadosReporte)
                        {
                            html += "<tr>";
                            html += $"<td>{contador}</td>";
                            html += $"<td>{emp.Nombres}</td>";
                            html += $"<td>{emp.DPI}</td>";
                            html += $"<td>{emp.Edad}</td>";
                            html += $"<td>{emp.Sexo}</td>";
                            html += $"<td>{emp.NombreDepartamento}</td>";
                            html += $"<td>{emp.FechaIngreso:dd/MM/yyyy}</td>";
                            html += $"<td>{CalcularTiempoLaborando(emp.FechaIngreso)}</td>";
                            html += $"<td>{(emp.Activo ? "Activo" : "Inactivo")}</td>";
                            html += "</tr>";
                            contador++;
                        }

                        html += "</table>";
                        html += "</body></html>";

                        Response.Write(html);


                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                    }
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception ex)
            {
                MostrarError($"Error al exportar a Excel: {ex.Message}");
            }
        }

        private void MostrarError(string mensaje)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "MostrarError",
                $"toastr.error('{mensaje.Replace("'", "\\'")}');", true);
        }

        private void MostrarExito(string mensaje)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "MostrarExito",
                $"toastr.success('{mensaje.Replace("'", "\\'")}');", true);
        }
    }
}