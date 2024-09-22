using System.Security.Cryptography.Xml;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
//builder.Configuration.AddXmlFile("appsettings.xml", optional: true, reloadOnChange: true);
//builder.Configuration.AddIniFile("appsettings.ini", optional: true, reloadOnChange: true);
builder.Configuration.AddJsonFile("aboutme.json", optional: true, reloadOnChange: true);
builder.Services.AddSingleton<ICompanyService, CompanyService>();

var app = builder.Build();

app.MapGet("/", (ICompanyService companyService) =>
{
    var company = companyService.GetCompanyWithMostEmployees();
    return Results.Ok($"Company with most employees: {company}");
});
app.MapGet("/my", (IConfiguration configuration) =>
{
    var myInfo = configuration.GetSection("AboutMe").Get<MyInfo>();
    return Results.Ok($"Name: {myInfo.Name}, Age: {myInfo.Age}, Hobbies: {string.Join(", ", myInfo.Hobbies)}");
});
app.Run();
public class MyInfo
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string Occupation { get; set; }
    public List<string> Hobbies { get; set; }
}
public interface ICompanyService
{
    string GetCompanyWithMostEmployees();
}
public class CompanyService : ICompanyService
{
    private readonly IConfiguration _configuration;

    public CompanyService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetCompanyWithMostEmployees()
    {
        var companies = _configuration.GetSection("Companies").GetChildren();
        string companyWithMostEmployees = null;
        int maxEmployees = 0;

        foreach (var company in companies)
        {
            int employees = int.Parse(company["Employees"]);
            if (employees > maxEmployees)
            {
                maxEmployees = employees;
                companyWithMostEmployees = company.Key;
            }
        }

        return companyWithMostEmployees;
    }
}
