using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Utilitar.AppContext;
using Utilitar.Dictionaries;
using Utilitar.Helpers.EmployeeHelper;
using Utilitar.Helpers.FreeDayHelper;
using Utilitar.Models;
using Utilitar.ViewModels;

namespace Utilitar.Helpers.ScheduleHelper
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly UtilitiesContext _context;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IFreeDayRepository _freeDayRepository;

        private readonly List<string> columns = new()
        {
            "a","b","c", "d","e","f", "g","h","i","j","k", "l", "m", "n", "o", "p","q", "r", "s", "t", "u", "v", "w", "x", "y", "z",
            "aa", "ab", "ac", "ad", "ae", "af", "ag", "ah", "ai","aj", "ak","al", "am", "an", "ao","ap","aq", "ar", "as", "at", "au","av", "aw", "ax", "ay"
        };
        //private readonly List<string> locations = new() {"PJ Targoviste", "PJ Gaesti", "PJ Racari", "PJ Moreni", "PJ Pucioasa"};
        public ScheduleRepository(UtilitiesContext context, IEmployeeRepository employeeRepository, IFreeDayRepository freeDayRepository)
        {
            _context = context;
            _employeeRepository = employeeRepository;
            _freeDayRepository = freeDayRepository;
        }
        
        /// <summary>
        /// Această metodă asincronă returnează o listă cu toate locațiile distincte ale angajaților dintr-o locație specificată.
        /// </summary>
        /// <param name="location">Locația specificată pentru a filtra angajații.</param>
        /// <returns>O listă de locații distincte ale angajaților din locația specificată.</returns>
        private async Task<List<string>> GetAllLocationsAsync(string location)
        {
            var employees = await _context.Employees
                            .Where(w => w.Location == location).ToListAsync();
            var result = employees.Select(s => s.PermanentLocation).Distinct().ToList();
            return result;
        }

        /// <summary>
        /// Această metodă asincronă returnează o listă cu toate zilele libere ale unui angajat, în funcție de ID-ul său.
        /// </summary>
        /// <param name="id">ID-ul angajatului pentru care se solicită zilele libere.</param>
        /// <returns>O listă de zile libere ale angajatului specificat.</returns>
        public async Task<IEnumerable<FreeDay>> GetDaysOff(int id)
        {
            var freeDay = await _context.FreeDays
                .Where(f => f.EmployeeId == id)
                .Include(f => f.DayOffType)
                .Include(f => f.EmployeeId) 
                .ToListAsync();
            return freeDay;
        }
        
        /// <summary>
        /// Verifică dacă o dată specificată este o zi legală liberă (sâmbătă, duminică sau sărbătoare legală).
        /// </summary>
        /// <param name="dateToCheck">Data de verificat pentru a determina dacă este o zi legală liberă.</param>
        /// <returns>True dacă data este o zi legală liberă, altfel False.</returns>

        public async Task<bool> IsLegallDayOff(DateTime dateToCheck)
        {
            // Interogarea bazei de date pentru a obține zilele legale libere din luna datei specificate.
            var legallDaysOff =  await _context.LegallDay.Where(d => d.DateOff.Month == dateToCheck.Month).ToListAsync(); 
            
            // Verifică dacă data este o zi de sâmbătă sau duminică.
            if (dateToCheck.DayOfWeek == DayOfWeek.Sunday || dateToCheck.DayOfWeek == DayOfWeek.Saturday)
            {
                return true;
            }
            // Verifică dacă data este o sărbătoare legală.
            foreach(var day in legallDaysOff)
            {
                if (day.DateOff.Day == dateToCheck.Day)
                {
                    return true;
                }
            }
            // Dacă data nu este nici sâmbătă, nici duminică și nici sărbătoare legală, atunci returnează False.
            return false;
        }

        /// <summary>
        /// Sterge un fișier dacă există.
        /// </summary>
        /// <param name="file">Fișierul care trebuie verificat și șters dacă există.</param>

        private static async Task  DeleteIfExists(FileInfo file)
        {
            if (file.Exists)
                file.Delete();
        }
        /// <summary>
        /// Calculează numarul de ore de concediu efectuat, in functie de tipul concediului.. Ex. CO, BO etc.
        /// </summary>
        /// <param name="dict">Dictionar .</param>
        /// <param name="crit">Criteriul pe baza căruia se calculează valoarea.</param>
        /// <returns>Valoarea calculată în funcție de numărul de apariții ale criteriului în listă.</returns>
        private static async Task<int> SetCellValue(List<string> dict, string crit)
        {
            int result = 0;
            foreach (var row in dict)
            {
                if (row == crit)
                    result += 8;
            }
            return result;
        }

        private static string GetMonthName (int i)
        {
            List<string> list = new()
            {
                "Ianuarie",
                "Februarie",
                "Martie",
                "Aprilie",
                "Mai",
                "Iunie",
                "Iulie",
                "August",
                "Septembrie",
                "Octombrie",
                "Noiembrie",
                "Decembrie"
            };

            return list[i - 1];
        }

        private static string GetHeaderLocation(string str)
        {
            string result = string.Empty;

            switch (str)
            {
                case "PJ Targoviste":
                    result = "Judecatoria Targoviste";
                    break;
                case "PJ Gaesti":
                    result = "Judecatoria Gaesti";
                    break;
                case "PJ Moreni":
                    result = "Judecatoria Moreni";
                    break;
                case "PJ Pucioasa":
                    result = "Judecatoria Pucioasa";
                    break;
                case "PJ Racari":
                    result = "Judecatoria Racari";
                    break;
                default:
                    result = "Tribunalul Dambovita";
                    break;
            }
            return result;  
        }

        private static void SetBorderAllignment(ExcelWorksheet worksheet, string cells, string val)
        {
            worksheet.Cells[cells].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            worksheet.Cells[cells].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            worksheet.Cells[cells].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
            worksheet.Cells[cells].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
            worksheet.Cells[cells].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
            worksheet.Cells[cells].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
            worksheet.Cells[cells].Value = val;
        }

        public async Task SaveExcelFile(FileInfo file, string sheetName, List<Employee> emp, ExportVM export)
        {
            var dateMonth = export.DateMonth;

            string monthName = GetMonthName(dateMonth);
            // institutia aleasa din formularul Export
            var location = export.Location;
            var daysInMonth = DateTime.DaysInMonth(export.DateYear, dateMonth);
            var delegateLocationList = await GetAllLocationsAsync(location);
            

            await DeleteIfExists(file);
            using var package = new ExcelPackage(file);

            foreach (var loc in delegateLocationList)
            {
                #region ################################################################ Create  Excel File ######################################################### 
                var empDelegate =  emp.Select(e => e)
                    .Where(e => e.Location == location && e.PermanentLocation == loc).ToList();
                if (empDelegate == null || empDelegate.Count < 1)
                    continue;
                sheetName = loc;
                var ws = package.Workbook.Worksheets.Add(sheetName);
                ws.View.ShowGridLines = false;
                ws.PrinterSettings.Orientation = eOrientation.Landscape;
                ws.PrinterSettings.PaperSize = ePaperSize.A4;
                ws.PrinterSettings.FitToPage = true;
                ws.PrinterSettings.RightMargin = 0;
                ws.PrinterSettings.LeftMargin = 0;


                ws.Column(2).Width = 2.86;
                ws.Column(3).Width = 30;
                ws.Column(5).Width = 12.71;

                int tempColumnContor = 5;
                int ctr = daysInMonth + tempColumnContor;
                for (int i = 6; i < ctr; i++)
                {
                    ws.Column(i).Width = 4;
                    tempColumnContor++;
                }

                ws.Column(tempColumnContor++).Width = 6.15;
                ws.Column(tempColumnContor++).Width = 8.3;
                ws.Column(tempColumnContor++).Width = 10.75;

                for (int i = tempColumnContor++; i < (tempColumnContor + 10); i++)
                {
                    ws.Column(i).Width = 4;
                }


                // antet stanga
                var antet1 = ws.Cells["a3:e3"];
                antet1.Value = "Parchetul de pe lângă".ToUpper();
                antet1.Merge = true;
                antet1.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                antet1.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                antet1.Style.Font.Size = 14;
                antet1.Style.Font.Bold = true;

                var antet2 = ws.Cells["a4:e4"];
                antet2.Value = GetHeaderLocation(export.Location).ToUpper();
                antet2.Merge = true;
                antet2.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                antet2.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                antet2.Style.Font.Size = 14;
                antet2.Style.Font.Bold = true;

                var antet3 = ws.Cells["a5:e5"];
                antet3.Value = $"nr. {export.RegNo}".ToUpper();
                antet3.Merge = true;
                antet3.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                antet3.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                antet3.Style.Font.Size = 14;
                antet3.Style.Font.Bold = true;

                //legenda
                var legendTitle = ws.Cells["ac3:as3"];
                legendTitle.Value = "Legendă".ToUpper();
                legendTitle.Merge = true;
                legendTitle.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                legendTitle.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                legendTitle.Style.Font.Size = 14;
                legendTitle.Style.Font.Bold = true;

                var co = ws.Cells["ad4:ak4"];
                co.Value = "Co-concediu de odihnă";
                co.Merge = true;
                co.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                co.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                co.Style.Font.Size = 12;
                co.Style.Font.Bold = true;

                var bo = ws.Cells["ad5:ak5"];
                bo.Value = "Bo-boală obişnuită";
                bo.Merge = true;
                bo.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                bo.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                bo.Style.Font.Size = 12;
                bo.Style.Font.Bold = true;

                var bp = ws.Cells["ad6:ak6"];
                bp.Value = "Bp-boală profesională";
                bp.Merge = true;
                bp.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                bp.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                bp.Style.Font.Size = 12;
                bp.Style.Font.Bold = true;

                var am = ws.Cells["ad7:ak7"];
                am.Value = "Am-accident de muncă";
                am.Merge = true;
                am.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                am.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                am.Style.Font.Size = 12;
                am.Style.Font.Bold = true;

                var m = ws.Cells["ad8:ak8"];
                m.Value = "Bo-boală obişnuită";
                m.Merge = true;
                m.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                m.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                m.Style.Font.Size = 12;
                m.Style.Font.Bold = true;

                var cfp = ws.Cells["al4:as4"];
                cfp.Value = "CFP-învoiri şi concediu fără plată";
                cfp.Merge = true;
                cfp.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                cfp.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                cfp.Style.Font.Size = 12;
                cfp.Style.Font.Bold = true;

                var o = ws.Cells["al5:as5"];
                o.Value = "O-obligaţii cetăţeneşti";
                o.Merge = true;
                o.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                o.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                o.Style.Font.Size = 12;
                o.Style.Font.Bold = true;

                var n = ws.Cells["al6:as6"];
                n.Value = "N-absenţă nemotivată";
                n.Merge = true;
                n.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                n.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                n.Style.Font.Size = 12;
                n.Style.Font.Bold = true;

                var cs = ws.Cells["al7:as7"];
                cs.Value = "Cs-concediu de studii";
                cs.Merge = true;
                cs.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                cs.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                cs.Style.Font.Size = 12;
                cs.Style.Font.Bold = true;

                var evd = ws.Cells["al4:as4"];
                evd.Value = "Evd-evenimente deosebite";
                evd.Merge = true;
                evd.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                evd.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                evd.Style.Font.Size = 12;
                evd.Style.Font.Bold = true;

                // title
                var title = ws.Cells["b12:aw12"];
                title.Value = "Foaie colectivă de prezenţă".ToUpper();
                title.Merge = true;
                title.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                title.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                title.Style.Font.Size = 20;
                title.Style.Font.Bold = true;

                var subtitle = ws.Cells["b14:aw14"];
                subtitle.Value = $"{monthName}   {export.DateYear}".ToUpper();
                subtitle.Merge = true;
                subtitle.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                subtitle.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                subtitle.Style.Font.Size = 20;
                subtitle.Style.Font.Bold = true;

                //table
                ws.Cells["b17"].Value = "Nr.crt.";
                var tableHeaderCrt = ws.Cells["b17:b19"];
                tableHeaderCrt.Merge = true;
                tableHeaderCrt.Style.Font.Size = 14;
                tableHeaderCrt.AutoFitColumns();
                tableHeaderCrt.Style.Font.Bold = true;
                tableHeaderCrt.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                tableHeaderCrt.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                tableHeaderCrt.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                tableHeaderCrt.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                tableHeaderCrt.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                tableHeaderCrt.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                var tableHeaderName = ws.Cells["c17:c19"];
                tableHeaderName.Value = "Numele şi prenumele";
                tableHeaderName.Style.Font.Size = 14;
                tableHeaderName.Style.Font.Bold = true;
                tableHeaderName.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                tableHeaderName.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                tableHeaderName.Merge = true;
                tableHeaderName.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                tableHeaderName.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                tableHeaderName.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                tableHeaderName.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;



                var tableHeaderMarca = ws.Cells["d17:d19"];
                tableHeaderMarca.Value = "Marca";
                tableHeaderCrt.AutoFitColumns();
                tableHeaderMarca.Merge = true;
                tableHeaderMarca.Style.Font.Size = 14;
                tableHeaderMarca.Style.Font.Bold = true;
                tableHeaderMarca.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                tableHeaderMarca.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                tableHeaderMarca.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                tableHeaderMarca.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                tableHeaderMarca.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                tableHeaderMarca.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                var tableHeaderRole = ws.Cells["e17:e19"];
                tableHeaderRole.Value = "Funcţia";
                tableHeaderRole.AutoFitColumns();
                tableHeaderRole.Merge = true;
                tableHeaderRole.Style.Font.Size = 14;
                tableHeaderRole.Style.Font.Bold = true;
                tableHeaderRole.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                tableHeaderRole.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                tableHeaderRole.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                tableHeaderRole.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                tableHeaderRole.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                tableHeaderRole.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                int tempIndex2 = daysInMonth + 5;
                var days = ws.Cells["f17:" + columns[tempIndex2 - 1] + "17"];
                days.Value = "zilele  lunii".ToUpper();
                days.Merge = true;
                days.Style.Font.Size = 14;
                days.Style.Font.Bold = true;
                days.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                days.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                days.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                days.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                days.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                days.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                var prez = ws.Cells["f18:" + columns[tempIndex2 - 1] + "18"];
                prez.Value = "modul de prezentare la serviciu".ToUpper();
                prez.Merge = true;
                prez.Style.Font.Size = 14;
                prez.Style.Font.Bold = true;
                prez.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                prez.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                prez.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                prez.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                prez.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                prez.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                for (int i = 5; i < tempIndex2; i++)
                {
                    string colName = columns[i] + "19";
                    var cell = ws.Cells[colName];
                    cell.Value = (i - 4).ToString();
                    cell.Style.Font.Size = 11;
                    cell.Style.Font.Bold = true;
                    cell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    cell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    cell.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    cell.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    cell.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    cell.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                }
                var secondCtr = tempIndex2;

                string colIndex = columns[secondCtr] + "17:" + columns[secondCtr] + "19";
                var totalWeekHours = ws.Cells[colIndex];
                totalWeekHours.Merge = true;
                totalWeekHours.Style.WrapText = true;
                totalWeekHours.Value = "total ore".ToUpper();
                totalWeekHours.AutoFitColumns();
                totalWeekHours.Style.Font.Size = 11;
                totalWeekHours.Style.Font.Bold = true;
                totalWeekHours.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                totalWeekHours.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                totalWeekHours.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                totalWeekHours.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                totalWeekHours.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                totalWeekHours.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                secondCtr++;
                colIndex = columns[secondCtr] + "17:" + columns[secondCtr] + "19";
                var workedHours = ws.Cells[colIndex];
                workedHours.AutoFitColumns();
                workedHours.Style.WrapText = true;
                workedHours.Value = "ore lucrate".ToUpper();
                workedHours.Merge = true;
                workedHours.Style.Font.Size = 11;
                workedHours.Style.Font.Bold = true;
                workedHours.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                workedHours.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                workedHours.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                workedHours.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                workedHours.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                workedHours.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                secondCtr++;
                colIndex = columns[secondCtr] + "17:" + columns[secondCtr] + "19";
                var missedHours = ws.Cells[colIndex];
                missedHours.AutoFitColumns();
                missedHours.Style.WrapText = true;
                missedHours.Value = "ore nelucrate".ToUpper();
                missedHours.Merge = true;
                missedHours.Style.Font.Size = 11;
                missedHours.Style.Font.Bold = true;
                missedHours.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                missedHours.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                missedHours.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                missedHours.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                missedHours.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                missedHours.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                secondCtr++;
                colIndex = columns[secondCtr] + "17:" + columns[secondCtr + 9] + "17";
                var fromWhich = ws.Cells[colIndex];
                fromWhich.Value = "din care".ToUpper();
                fromWhich.Merge = true;
                fromWhich.Style.Font.Size = 11;
                fromWhich.Style.Font.Bold = true;
                fromWhich.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                fromWhich.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                fromWhich.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                fromWhich.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                fromWhich.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                fromWhich.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                colIndex = columns[secondCtr] + "18:" + columns[secondCtr] + "19";
                var coTable = ws.Cells[colIndex];
                coTable.Value = "Co";
                coTable.Merge = true;
                coTable.Style.Font.Size = 12;
                coTable.Style.Font.Bold = true;
                coTable.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                coTable.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                coTable.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                coTable.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                coTable.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                coTable.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                secondCtr++;
                colIndex = columns[secondCtr] + "18:" + columns[secondCtr] + "19";
                var boTable = ws.Cells[colIndex];
                boTable.Value = "Bo";
                boTable.Merge = true;
                boTable.Style.Font.Size = 12;
                boTable.Style.Font.Bold = true;
                boTable.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                boTable.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                boTable.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                boTable.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                boTable.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                boTable.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                secondCtr++;
                colIndex = columns[secondCtr] + "18:" + columns[secondCtr] + "19";
                var bpTable = ws.Cells[colIndex];
                bpTable.Value = "Bp";
                bpTable.Merge = true;
                bpTable.Style.Font.Size = 12;
                bpTable.Style.Font.Bold = true;
                bpTable.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                bpTable.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                bpTable.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                bpTable.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                bpTable.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                bpTable.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                secondCtr++;
                colIndex = columns[secondCtr] + "18:" + columns[secondCtr] + "19";
                var amTable = ws.Cells[colIndex];
                amTable.Value = "Am";
                amTable.Merge = true;
                amTable.Style.Font.Size = 12;
                amTable.Style.Font.Bold = true;
                amTable.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                amTable.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                amTable.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                amTable.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                amTable.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                amTable.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                secondCtr++;
                colIndex = columns[secondCtr] + "18:" + columns[secondCtr] + "19";
                var mTable = ws.Cells[colIndex];
                mTable.Value = "M";
                mTable.Merge = true;
                mTable.Style.Font.Size = 12;
                mTable.Style.Font.Bold = true;
                mTable.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                mTable.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                mTable.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                mTable.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                mTable.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                mTable.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                secondCtr++;
                colIndex = columns[secondCtr] + "18:" + columns[secondCtr] + "19";
                var cfpTable = ws.Cells[colIndex];
                cfpTable.Value = "CFP";
                cfpTable.Merge = true;
                cfpTable.Style.Font.Size = 12;
                cfpTable.Style.Font.Bold = true;
                cfpTable.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                cfpTable.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                cfpTable.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                cfpTable.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                cfpTable.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                cfpTable.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                secondCtr++;
                colIndex = columns[secondCtr] + "18:" + columns[secondCtr] + "19";
                var oTable = ws.Cells[colIndex];
                oTable.Value = "O";
                oTable.Merge = true;
                oTable.Style.Font.Size = 12;
                oTable.Style.Font.Bold = true;
                oTable.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                oTable.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                oTable.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                oTable.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                oTable.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                oTable.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                secondCtr++;
                colIndex = columns[secondCtr] + "18:" + columns[secondCtr] + "19";
                var nTable = ws.Cells[colIndex];
                nTable.Value = "N".ToUpper();
                nTable.Merge = true;
                nTable.Style.Font.Size = 12;
                nTable.Style.Font.Bold = true;
                nTable.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                nTable.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                nTable.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                nTable.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                nTable.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                nTable.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                secondCtr++;
                colIndex = columns[secondCtr] + "18:" + columns[secondCtr] + "19";
                var csTable = ws.Cells[colIndex];
                csTable.Value = "Cs";
                csTable.Merge = true;
                csTable.Style.Font.Size = 12;
                csTable.Style.Font.Bold = true;
                csTable.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                csTable.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                csTable.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                csTable.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                csTable.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                csTable.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                secondCtr++;
                colIndex = columns[secondCtr] + "18:" + columns[secondCtr] + "19";
                var evdTable = ws.Cells[colIndex];
                evdTable.Value = "Evd";
                evdTable.Merge = true;
                evdTable.Style.Font.Size = 12;
                evdTable.Style.Font.Bold = true;
                evdTable.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                evdTable.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                evdTable.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                evdTable.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                evdTable.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                evdTable.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                #endregion

                #region ############################################################### Insert Rows and Data ##############################################################
                var listOfRoles = await _context.Roles.ToListAsync();
                //var specialistIT = emp.Where(x => x.Role == "Specialist IT" && x.PermanentLocation == location);

                // extrage toate sarbatorile legale din luna selectata
                var legallDaysList = await _context.LegallDay.Where(d => d.DateOff.Month == dateMonth).ToListAsync();
                
                // intocmeste o lista doar cu ziua din datele selectate mai sus
                var daysList = legallDaysList.Select(d => d.DateOff.Day).ToList();
               

                //intocmeste o lista cu zilele de weekend din luna si anul selectat
                var monthDaysList = Enumerable.Range(1, DateTime.DaysInMonth(export.DateYear, dateMonth))  // Days: 1, 2 ... 31 etc.
                        .Select(day => new DateTime(export.DateYear, dateMonth, day))
                        .Where(d => d.DayOfWeek.Equals(DayOfWeek.Saturday) || d.DayOfWeek.Equals(DayOfWeek.Sunday)) // Map each day to a date
                        .ToList();
                var weekendDaysList = monthDaysList.Select(d => d.Day).ToList();

                daysList = daysList.Except(weekendDaysList).ToList();

                
                

                // se foloseste pentru stabilirea numarului de criteriu din tabelul excel
                int crt = 0;
                //se foloseste pentru alegerea randului din foaia de lucru
                int rowNumber = 20;


                 foreach (var role in listOfRoles)
                 {

                    var rl = emp.Where(x => x.RoleId == role.Id && (x.Location == location && x.PermanentLocation == loc)) 
                        .OrderBy(o => o.Roles.PriorityNo).ToList();

                
                    if (rl.Any())
                    {
                        foreach (var p in rl)
                        {
                            int totalHours = 0;
                            //int hoursMonthly = 0;
                            int colNumber = 1;
                            crt++;

                            var daysOffList = await _context.FreeDays.Where(d => d.EmployeeId == p.Id 
                                                                            && (d.StartDate.Month == dateMonth || d.FinishDate.Month == dateMonth) 
                                                                            && d.Employee.PermanentLocation == loc && d.Employee.Location == location)
                                                                      .Include(x => x.DayOffType)
                                                                      .Include(e => e.Employee)
                                                                      .ToListAsync();


                            List<string> daysOffDictionary = new();
                            List<int> freedays = new();

                            foreach (var day in daysOffList)
                            {

                                if (day.StartDate.Month == day.FinishDate.Month)
                                {
                                    for (int i = day.StartDate.Day; i <= day.FinishDate.Day; i++)
                                    {
                                        freedays.Add(i);
                                        if (!weekendDaysList.Contains(i) && !daysList.Contains(i))
                                            daysOffDictionary.Add(day.DayOffType.Description);
                                    }
                                }
                                else if (day.StartDate.Month < dateMonth)
                                {
                                    for (int i = 1; i <= day.FinishDate.Day; i++)
                                    {
                                        freedays.Add(i);
                                        if (!weekendDaysList.Contains(i) && !daysList.Contains(i))
                                            daysOffDictionary.Add(day.DayOffType.Description);
                                    }
                                }
                                else
                                {
                                    for (int i = day.StartDate.Day; i <= DateTime.DaysInMonth(export.DateYear, dateMonth); i++)
                                    {
                                        freedays.Add(i);
                                        if (!weekendDaysList.Contains(i) && !daysList.Contains(i))
                                            daysOffDictionary.Add(day.DayOffType.Description);
                                    }
                                }

                            };

                            string cellRange = columns[colNumber] + rowNumber.ToString();
                            SetBorderAllignment(ws, cellRange, crt.ToString());
                            ws.Cells[cellRange].Style.Font.Size = 12;
                            colNumber++;

                            cellRange = columns[colNumber] + rowNumber.ToString();
                            SetBorderAllignment(ws, cellRange, p.LastName + " " + p.FirstName);
                            ws.Cells[cellRange].Style.Font.Size = 18;
                            colNumber++;

                            if (!String.IsNullOrEmpty(p.Marca))
                            {
                                cellRange = columns[colNumber] + rowNumber.ToString();
                                SetBorderAllignment(ws, cellRange, p.Marca);
                                ws.Cells[cellRange].Style.Font.Size = 12;
                                colNumber++;
                            }

                            else
                            {
                                cellRange = columns[colNumber] + rowNumber.ToString();
                                SetBorderAllignment(ws, cellRange, string.Empty);
                                colNumber++;
                            }

                            cellRange = columns[colNumber] + rowNumber.ToString();
                            SetBorderAllignment(ws, cellRange, p.Roles.RoleName);
                            ws.Cells[cellRange].Style.Font.Size = 12;
                            colNumber++;

                            // contorizeaza zilele lipsa pentru luna cand a inceput sau a terminat
                            int numberOfDays = 0;

                            for (int i = colNumber; i < colNumber + daysInMonth; i++)
                            {
                                var tempCell = ws.Cells[columns[i] + rowNumber.ToString()];
                                // verifica daca ziua este libera, este zi de weekend sau este mai mare decat
                                if (daysList.Contains(i - 4) || weekendDaysList.Contains(i - 4) || (i - 4) > DateTime.DaysInMonth(export.DateYear, dateMonth))
                                {
                                    ws.Cells[columns[i] + (rowNumber - 1).ToString()].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    ws.Cells[columns[i] + (rowNumber - 1).ToString()].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                                    tempCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    tempCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                                    SetBorderAllignment(ws, columns[i] + rowNumber.ToString(), string.Empty);
                                }
                                
                                else if (p.DataStart.Year >= export.DateYear && p.DataStart.Month == export.DateMonth && p.DataStart.Day > i-4)
                                {
                                    tempCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    tempCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                                    SetBorderAllignment(ws, columns[i] + rowNumber.ToString(), string.Empty);
                                    numberOfDays += 1;
                                }
                                else if (p.DataFinal.Year <= export.DateYear && p.DataFinal.Month == export.DateMonth && p.DataFinal.Day < i-4)
                                {
                                    tempCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    tempCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                                    SetBorderAllignment(ws, columns[i] + rowNumber.ToString(), string.Empty);
                                    numberOfDays += 1;
                                }
                                else if (freedays.Contains(i - 4))
                                {
                                    var val = await _freeDayRepository.GetDayDescription(p.Id, i - 4, dateMonth, export.DateYear);
                                    tempCell.Style.Font.Size = 18;
                                    //tempCell.Style.Font.Bold = true;
                                    SetBorderAllignment(ws, columns[i] + rowNumber.ToString(), val);
                                }
                                else
                                {
                                    tempCell.Style.Font.Size = 18;
                                    SetBorderAllignment(ws, columns[i] + rowNumber.ToString(), 8.ToString());
                                    totalHours += 8;
                                }
                            }
                            colNumber += daysInMonth;
                            int hours = 0;

                            foreach (var day in monthDaysList)
                            {
                                if (day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday)
                                    hours++;
                            }
                            int totalOre = (DateTime.DaysInMonth(export.DateYear, dateMonth) - hours - daysList.Count - numberOfDays) * 8;

                            cellRange = columns[colNumber] + rowNumber.ToString();
                            SetBorderAllignment(ws, cellRange, totalOre.ToString());
                            ws.Cells[cellRange].Style.Font.Size = 18;
                            colNumber++;

                            cellRange = columns[colNumber] + rowNumber.ToString();
                            SetBorderAllignment(ws, cellRange, totalHours.ToString());
                            ws.Cells[cellRange].Style.Font.Size = 18;
                            colNumber++;

                            ws.Cells[columns[colNumber] + rowNumber.ToString()].Value = (totalOre - totalHours).ToString();
                            cellRange = columns[colNumber] + rowNumber.ToString();
                            SetBorderAllignment(ws, cellRange, (totalOre - totalHours).ToString());
                            ws.Cells[cellRange].Style.Font.Size = 18;
                            colNumber++;

                            var tempVal = await SetCellValue(daysOffDictionary, "CO");
                            cellRange = columns[colNumber] + rowNumber.ToString();
                            SetBorderAllignment(ws, cellRange, tempVal.ToString());
                            ws.Cells[cellRange].Style.Font.Size = 18;
                            colNumber++;

                            tempVal = await SetCellValue(daysOffDictionary, "BO");
                            cellRange = columns[colNumber] + rowNumber.ToString();
                            SetBorderAllignment(ws, cellRange, tempVal.ToString());
                            ws.Cells[cellRange].Style.Font.Size = 18;
                            colNumber++;

                            tempVal = await SetCellValue(daysOffDictionary, "BP");
                            cellRange = columns[colNumber] + rowNumber.ToString();
                            SetBorderAllignment(ws, cellRange, tempVal.ToString());
                            ws.Cells[cellRange].Style.Font.Size = 18;
                            colNumber++;

                            tempVal = await SetCellValue(daysOffDictionary, "AM");
                            cellRange = columns[colNumber] + rowNumber.ToString();
                            SetBorderAllignment(ws, cellRange, tempVal.ToString());
                            ws.Cells[cellRange].Style.Font.Size = 18;
                            colNumber++;

                            tempVal = await SetCellValue(daysOffDictionary, "M");
                            cellRange = columns[colNumber] + rowNumber.ToString();
                            SetBorderAllignment(ws, cellRange, tempVal.ToString());
                            ws.Cells[cellRange].Style.Font.Size = 18;
                            colNumber++;

                            tempVal = await SetCellValue(daysOffDictionary, "CFP");
                            cellRange = columns[colNumber] + rowNumber.ToString();
                            SetBorderAllignment(ws, cellRange, tempVal.ToString());
                            ws.Cells[cellRange].Style.Font.Size = 18;
                            colNumber++;

                            tempVal = await SetCellValue(daysOffDictionary, "O");
                            cellRange = columns[colNumber] + rowNumber.ToString();
                            SetBorderAllignment(ws, cellRange, tempVal.ToString());
                            ws.Cells[cellRange].Style.Font.Size = 18;
                            colNumber++;

                            tempVal = await SetCellValue(daysOffDictionary, "N");
                            cellRange = columns[colNumber] + rowNumber.ToString();
                            SetBorderAllignment(ws, cellRange, tempVal.ToString());
                            ws.Cells[cellRange].Style.Font.Size = 18;
                            colNumber++;

                            tempVal = await SetCellValue(daysOffDictionary, "CS");
                            cellRange = columns[colNumber] + rowNumber.ToString();
                            SetBorderAllignment(ws, cellRange, tempVal.ToString());
                            ws.Cells[cellRange].Style.Font.Size = 18;
                            colNumber++;

                            tempVal = await SetCellValue(daysOffDictionary, "EVD");
                            cellRange = columns[colNumber] + rowNumber.ToString();
                            SetBorderAllignment(ws, cellRange, tempVal.ToString());
                            ws.Cells[cellRange].Style.Font.Size = 18;
                            colNumber++;
                            rowNumber++;


                            #endregion
                        }
                    }
                
                 }
                rowNumber += 2;
                var semnat = ws.Cells[$"d{rowNumber}:q{rowNumber}"];
                semnat.Value = "PRIM PROCUROR";
                semnat.Merge = true;
                semnat.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                semnat.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                semnat.Style.Font.Size = 14;
                semnat.Style.Font.Bold = true;

                var intocmit = ws.Cells[$"al{rowNumber}:av{rowNumber}"];
                intocmit.Value = "INTOCMIT,";
                intocmit.Merge = true;
                intocmit.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                intocmit.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                intocmit.Style.Font.Size = 14;
                intocmit.Style.Font.Bold = true;

                rowNumber += 2;
                var semnatBy = ws.Cells[$"d{rowNumber}:q{rowNumber}"];
                semnatBy.Value = export.SignedBy.ToUpper();
                semnatBy.Merge = true;
                semnatBy.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                semnatBy.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                semnatBy.Style.Font.Size = 14;
                semnatBy.Style.Font.Bold = true;

                var intocmitBy = ws.Cells[$"al{rowNumber}:av{rowNumber}"];
                intocmitBy.Value = export.MadeBy.ToUpper();
                intocmitBy.Merge = true;
                intocmitBy.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                intocmitBy.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                intocmitBy.Style.Font.Size = 14;
                intocmitBy.Style.Font.Bold = true;

            }
            await package.SaveAsync();
            
        }

    }
}
