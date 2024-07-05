using AutoMapper;
using HealthTracking.Configration.Messages;
using HealthTracking.DataService.IConfigration;
using HealthTracking.Entity.DbSet;
using HealthTracking.Entity.Dtos.Generic;
using HealthTracking.Entity.Dtos.Incoming;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.XlsIO;
using Syncfusion.XlsIORenderer;
using System.Security.Claims;

namespace HealthTracking.Api.Controllers.v1
{
    //[Authorize]
    public class HealthController : BaseController
    {
        public HealthController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, IMapper mapper) : base(unitOfWork, userManager, mapper) { }
        
        
        //TODO Add Admin role
        [HttpGet]
        public async Task<IActionResult> GetHealthData()
        {
            var result = new PagedResult<HealthData>();

            var HealthData = await _unitOfWork.HealthData.GetAll();
            
            // {try catch} => catch returns a null
            if (HealthData == null)
            {
                result.Error = PopulateError(500, ErrorsMessages.Generic.SomethingWentWrong, ErrorsMessages.Generic.TypeInternalServerError);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            result.Content = HealthData.ToList();
            result.ResultCount = HealthData.Count();
            return Ok(result);
        }


        [HttpGet]
        [Route("UserHealthData", Name = "GetUserHealthData")]
        public async Task<IActionResult> GetUserHealthData()
        {
            var result = new PagedResult<HealthData>();

            //HttpContext.User
            // any Request with a Jwt token The EF will process the Token and Do some verification then attach a User object based on the token to HttpContext
            var userId = await GetUserIdByUserClaim(HttpContext.User);

            if (userId == Guid.Empty)
            {
                result.Error = PopulateError(400, ErrorsMessages.profile.UserNotFound, ErrorsMessages.Generic.TypeBadRequest);
                return BadRequest(result);
            }

            var healthData = await _unitOfWork.HealthData.GetUserHealthData(userId);

            if(healthData == null)
            {
                result.Error = PopulateError(500, ErrorsMessages.Generic.SomethingWentWrong, ErrorsMessages.Generic.TypeInternalServerError);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            result.Content = healthData;
            result.ResultCount = healthData.Count;

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add(HealthData healthData)
        {
            var result = new Result<HealthData>();

            var userId = await GetUserIdByUserClaim(HttpContext.User);

            if(userId == Guid.Empty)
            {
                result.Error = PopulateError(400, ErrorsMessages.profile.UserNotFound, ErrorsMessages.Generic.TypeBadRequest);
                return BadRequest(result);
            }

            healthData.userId = userId;

            bool added = await _unitOfWork.HealthData.Add(healthData);

            if (!added)
            {
                result.Error = PopulateError(500, ErrorsMessages.Generic.SomethingWentWrong, ErrorsMessages.Generic.TypeInternalServerError);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            await _unitOfWork.CompleteAsync();

            result.Content = healthData;

            return CreatedAtRoute("GetUserHealthData", new { id = healthData.Id }, result);
        }

        [HttpPut]
        public async Task<IActionResult> Update(HealthData healthData)
        {
            // check for the user
            var result = new Result<HealthData>();

            var userId = await GetUserIdByUserClaim(HttpContext.User);

            if (userId == Guid.Empty)
            {
                result.Error = PopulateError(400, ErrorsMessages.profile.UserNotFound, ErrorsMessages.Generic.TypeBadRequest);
                return BadRequest(result);
            }


            var isUpdated = await _unitOfWork.HealthData.UpdateHealthData(healthData);
            if (!isUpdated)
            {
                result.Error = PopulateError(500, ErrorsMessages.Generic.SomethingWentWrong, ErrorsMessages.Generic.TypeInternalServerError);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            await _unitOfWork.CompleteAsync();

            result.Content = healthData;
            return Ok(result);
        }

        [HttpGet]
        [Route("Report")]
        public async Task<IActionResult> GetReport()
        {
            var result = new Result<HealthData>();
            var userId = await GetUserIdByUserClaim(HttpContext.User);

            if (userId == Guid.Empty)
            {
                result.Error = PopulateError(400, ErrorsMessages.profile.UserNotFound, ErrorsMessages.Generic.TypeBadRequest);
                return BadRequest(result);
            }

            var healthData = await _unitOfWork.HealthData.GetUserHealthData(userId);

            if (healthData == null)
            {
                result.Error = PopulateError(500, ErrorsMessages.Generic.SomethingWentWrong, ErrorsMessages.Generic.TypeInternalServerError);
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Excel2013;
                IWorkbook workbook = application.Workbooks.Create(1);

                IWorksheet sheet = workbook.Worksheets[0];

                //Inserts the sample data for the chart
                sheet.Range["A1"].Text = "Time";
                sheet.Range["B1"].Text = "Blood Presure";
                sheet.Range["C1"].Text = "Boold Suger Level";
                sheet.Range["D1"].Text = "Height";
                sheet.Range["E1"].Text = "Wieght";

                for (int i = 0; i < healthData.Count; i++)
                {
                    var time = healthData[i].AddedDate.Date.ToString("dd/M/yyyy");
                    sheet.Range[$"A{i+2}"].Text = time;
                }

                int endRange = healthData.Count + 1;

                //Set Data
                for (int i = 2; i <= endRange; i++) // date
                {
                    var Presure = healthData[i - 2].BloodPresure;
                    var Suger = healthData[i - 2].BooldSugerLevel;
                    var H = healthData[i - 2].Height;
                    var W = healthData[i - 2].Wieght;

                    sheet.Range[i, 2].Number = Decimal.ToDouble(Presure);
                    sheet.Range[i, 3].Number = Decimal.ToDouble(Suger);
                    sheet.Range[i, 4].Number = Decimal.ToDouble(H);
                    sheet.Range[i, 5].Number = Decimal.ToDouble(W);
                }

                IChartShape chart = sheet.Charts.Add();

                //Set chart type
                chart.ChartType = ExcelChartType.Line;

                //Set Chart Title
                chart.ChartTitle = "Health Traking Report";

                //Set BloodPresure serie
                IChartSerie BloodPresure = chart.Series.Add("BloodPresure");
                BloodPresure.Values = sheet.Range[$"B2:B{endRange}"]; // data
                BloodPresure.CategoryLabels = sheet.Range[$"A2:A{endRange}"]; // time

                //Set BooldSugerLevel serie
                IChartSerie BooldSugerLevel = chart.Series.Add("BooldSugerLevel");
                BooldSugerLevel.Values = sheet.Range[$"C2:C{endRange}"]; // data
                BooldSugerLevel.CategoryLabels = sheet.Range[$"A2:A{endRange}"]; // time

                //Set first serie
                IChartSerie Height = chart.Series.Add("Height");
                Height.Values = sheet.Range[$"D2:D{endRange}"]; // data
                Height.CategoryLabels = sheet.Range[$"A2:A{endRange}"]; // time

                //Set first serie
                IChartSerie Wieght = chart.Series.Add("Wieght");
                Wieght.Values = sheet.Range[$"E2:E{endRange}"]; // data
                Wieght.CategoryLabels = sheet.Range[$"A2:A{endRange}"]; // time

                //Initialize XlsIO renderer.
                XlsIORenderer renderer = new XlsIORenderer();

                //Convert Excel document into PDF document 
                Syncfusion.Pdf.PdfDocument pdfDocument = renderer.ConvertToPDF(sheet);

                MemoryStream stream = new MemoryStream();
                pdfDocument.Save(stream);

                stream.Flush(); //Always catches me out
                stream.Position = 0; //Not sure if this is required
                                     //stream.Dispose();

                return File(stream, "application/pdf", "chart.pdf");
            }
        }

        private async Task<Guid> GetUserIdByUserClaim(ClaimsPrincipal userClaim)
        {
            var loggedInUser = await _userManager.GetUserAsync(userClaim);
            if (loggedInUser == null)
            {
                return Guid.Empty;
            }

            var identityId = new Guid(loggedInUser.Id);

            var user = await _unitOfWork.Users.GetByIdentityId(identityId);

            if (user == null)
            {
                return Guid.Empty;
            }

            return user.Id;
        }
    }
}
