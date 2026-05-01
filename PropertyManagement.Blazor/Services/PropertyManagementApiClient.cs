using System.Net.Http.Json;
using PropertyManagement.Blazor.Models;

namespace PropertyManagement.Blazor.Services;

public class PropertyManagementApiClient(HttpClient httpClient)
{
    public HttpClient HttpClient => httpClient;

    public async Task<List<PropertyOption>> GetPropertiesAsync() =>
        await httpClient.GetFromJsonAsync<List<PropertyOption>>("api/properties") ?? [];

    public async Task<PropertyOption> CreatePropertyAsync(PropertyFormModel form)
    {
        var response = await httpClient.PostAsJsonAsync("api/properties", form);
        return await ReadAsync<PropertyOption>(response);
    }

    public async Task<PropertyOption> UpdatePropertyAsync(int id, PropertyFormModel form)
    {
        var response = await httpClient.PutAsJsonAsync($"api/properties/{id}", form);
        return await ReadAsync<PropertyOption>(response);
    }

    public async Task DeletePropertyAsync(int id)
    {
        var response = await httpClient.DeleteAsync($"api/properties/{id}");
        await EnsureSuccessAsync(response);
    }

    public async Task<List<TenantModel>> GetTenantsAsync() =>
        await httpClient.GetFromJsonAsync<List<TenantModel>>("api/tenants") ?? [];

    public async Task<TenantModel> CreateTenantAsync(TenantFormModel form)
    {
        var response = await httpClient.PostAsJsonAsync("api/tenants", form);
        return await ReadAsync<TenantModel>(response);
    }

    public async Task<TenantModel> UpdateTenantAsync(int id, TenantFormModel form)
    {
        var response = await httpClient.PutAsJsonAsync($"api/tenants/{id}", form);
        return await ReadAsync<TenantModel>(response);
    }

    public async Task DeleteTenantAsync(int id)
    {
        var response = await httpClient.DeleteAsync($"api/tenants/{id}");
        await EnsureSuccessAsync(response);
    }

    public async Task<List<RentScheduleModel>> GetRentSchedulesAsync(string? status = null, string? month = null, int? tenantId = null, int? propertyId = null)
    {
        var endpoint = BuildQuery(
            "api/rentschedules",
            ("status", status),
            ("month", month),
            ("tenantId", tenantId?.ToString()),
            ("propertyId", propertyId?.ToString()));

        return await httpClient.GetFromJsonAsync<List<RentScheduleModel>>(endpoint) ?? [];
    }

    public async Task<RentScheduleModel> CreateRentScheduleAsync(RentScheduleFormModel form)
    {
        var response = await httpClient.PostAsJsonAsync("api/rentschedules", form);
        return await ReadAsync<RentScheduleModel>(response);
    }

    public async Task<RentScheduleModel> UpdateRentScheduleAsync(int id, RentScheduleFormModel form)
    {
        var response = await httpClient.PutAsJsonAsync($"api/rentschedules/{id}", form);
        return await ReadAsync<RentScheduleModel>(response);
    }

    public async Task<RentScheduleModel> ApplyLateFeeAsync(int id, ApplyLateFeeModel form)
    {
        var response = await httpClient.PostAsJsonAsync($"api/rentschedules/{id}/apply-late-fee", form);
        return await ReadAsync<RentScheduleModel>(response);
    }

    public async Task DeleteRentScheduleAsync(int id)
    {
        var response = await httpClient.DeleteAsync($"api/rentschedules/{id}");
        await EnsureSuccessAsync(response);
    }

    public async Task<List<RentPayment>> GetRentPaymentsAsync(int? tenantId = null, int? scheduleId = null)
    {
        var endpoint = BuildQuery(
            "api/rentpayments",
            ("tenantId", tenantId?.ToString()),
            ("scheduleId", scheduleId?.ToString()));

        return await httpClient.GetFromJsonAsync<List<RentPayment>>(endpoint) ?? [];
    }

    public async Task<RentPayment> CreateRentPaymentAsync(RentPaymentFormModel form)
    {
        var response = await httpClient.PostAsJsonAsync("api/rentpayments", form);
        return await ReadAsync<RentPayment>(response);
    }

    public async Task<RentPayment> UpdateRentPaymentAsync(int id, RentPaymentFormModel form)
    {
        var response = await httpClient.PutAsJsonAsync($"api/rentpayments/{id}", form);
        return await ReadAsync<RentPayment>(response);
    }

    public async Task DeleteRentPaymentAsync(int id)
    {
        var response = await httpClient.DeleteAsync($"api/rentpayments/{id}");
        await EnsureSuccessAsync(response);
    }

    public async Task<List<CommunicationLogModel>> GetCommunicationLogsAsync(int? tenantId = null, int? scheduleId = null)
    {
        var endpoint = BuildQuery(
            "api/communicationlogs",
            ("tenantId", tenantId?.ToString()),
            ("scheduleId", scheduleId?.ToString()));

        return await httpClient.GetFromJsonAsync<List<CommunicationLogModel>>(endpoint) ?? [];
    }

    public async Task<CommunicationLogModel> CreateCommunicationLogAsync(CommunicationLogFormModel form)
    {
        var response = await httpClient.PostAsJsonAsync("api/communicationlogs", form);
        return await ReadAsync<CommunicationLogModel>(response);
    }

    public async Task<TenantLedgerModel?> GetTenantLedgerAsync(int tenantId) =>
        await httpClient.GetFromJsonAsync<TenantLedgerModel>($"api/rentrecords/tenant/{tenantId}");

    public async Task<PropertyLedgerModel?> GetPropertyLedgerAsync(int propertyId) =>
        await httpClient.GetFromJsonAsync<PropertyLedgerModel>($"api/rentrecords/property/{propertyId}");

    public async Task<List<MaintenanceProjectModel>> GetMaintenanceProjectsAsync(int? propertyId = null, string? status = null)
    {
        var endpoint = BuildQuery(
            "api/maintenanceprojects",
            ("propertyId", propertyId?.ToString()),
            ("status", status));

        return await httpClient.GetFromJsonAsync<List<MaintenanceProjectModel>>(endpoint) ?? [];
    }

    public async Task<MaintenanceProjectModel> CreateMaintenanceProjectAsync(MaintenanceProjectFormModel form)
    {
        var response = await httpClient.PostAsJsonAsync("api/maintenanceprojects", form);
        return await ReadAsync<MaintenanceProjectModel>(response);
    }

    public async Task<MaintenanceProjectModel> UpdateMaintenanceProjectAsync(int id, MaintenanceProjectFormModel form)
    {
        var response = await httpClient.PutAsJsonAsync($"api/maintenanceprojects/{id}", form);
        return await ReadAsync<MaintenanceProjectModel>(response);
    }

    public async Task DeleteMaintenanceProjectAsync(int id)
    {
        var response = await httpClient.DeleteAsync($"api/maintenanceprojects/{id}");
        await EnsureSuccessAsync(response);
    }

    public async Task<List<WorkLogModel>> GetWorkLogsAsync(int? projectId = null)
    {
        var endpoint = BuildQuery("api/worklogs", ("projectId", projectId?.ToString()));
        return await httpClient.GetFromJsonAsync<List<WorkLogModel>>(endpoint) ?? [];
    }

    public async Task<WorkLogModel> CreateWorkLogAsync(WorkLogFormModel form)
    {
        var response = await httpClient.PostAsJsonAsync("api/worklogs", form);
        return await ReadAsync<WorkLogModel>(response);
    }

    public async Task<WorkLogModel> UpdateWorkLogAsync(int id, WorkLogFormModel form)
    {
        var response = await httpClient.PutAsJsonAsync($"api/worklogs/{id}", form);
        return await ReadAsync<WorkLogModel>(response);
    }

    public async Task DeleteWorkLogAsync(int id)
    {
        var response = await httpClient.DeleteAsync($"api/worklogs/{id}");
        await EnsureSuccessAsync(response);
    }

    public async Task<List<InvoiceModel>> GetInvoicesAsync(string? status = null, int? projectId = null, int? scheduleId = null, int? tenantId = null)
    {
        var endpoint = BuildQuery(
            "api/invoices",
            ("status", status),
            ("projectId", projectId?.ToString()),
            ("scheduleId", scheduleId?.ToString()),
            ("tenantId", tenantId?.ToString()));

        return await httpClient.GetFromJsonAsync<List<InvoiceModel>>(endpoint) ?? [];
    }

    public async Task<InvoiceModel> CreateInvoiceAsync(InvoiceFormModel form)
    {
        var response = await httpClient.PostAsJsonAsync("api/invoices", form);
        return await ReadAsync<InvoiceModel>(response);
    }

    public async Task<InvoiceModel> UpdateInvoiceAsync(int id, InvoiceFormModel form)
    {
        var response = await httpClient.PutAsJsonAsync($"api/invoices/{id}", form);
        return await ReadAsync<InvoiceModel>(response);
    }

    public async Task DeleteInvoiceAsync(int id)
    {
        var response = await httpClient.DeleteAsync($"api/invoices/{id}");
        await EnsureSuccessAsync(response);
    }

    public async Task<List<EvictionCaseModel>> GetEvictionCasesAsync(string? status = null, int? tenantId = null)
    {
        var endpoint = BuildQuery(
            "api/evictioncases",
            ("status", status),
            ("tenantId", tenantId?.ToString()));

        return await httpClient.GetFromJsonAsync<List<EvictionCaseModel>>(endpoint) ?? [];
    }

    public async Task<EvictionCaseModel> CreateEvictionCaseAsync(EvictionCaseFormModel form)
    {
        var response = await httpClient.PostAsJsonAsync("api/evictioncases", form);
        return await ReadAsync<EvictionCaseModel>(response);
    }

    public async Task<EvictionCaseModel> UpdateEvictionCaseAsync(int id, EvictionCaseFormModel form)
    {
        var response = await httpClient.PutAsJsonAsync($"api/evictioncases/{id}", form);
        return await ReadAsync<EvictionCaseModel>(response);
    }

    public async Task DeleteEvictionCaseAsync(int id)
    {
        var response = await httpClient.DeleteAsync($"api/evictioncases/{id}");
        await EnsureSuccessAsync(response);
    }

    public async Task<List<PropertyApplicationModel>> GetPropertyApplicationsAsync(string? status = null, int? propertyId = null)
    {
        var endpoint = BuildQuery(
            "api/propertyapplications",
            ("status", status),
            ("propertyId", propertyId?.ToString()));

        return await httpClient.GetFromJsonAsync<List<PropertyApplicationModel>>(endpoint) ?? [];
    }

    public async Task<PropertyApplicationModel> CreatePropertyApplicationAsync(PropertyApplicationFormModel form)
    {
        var response = await httpClient.PostAsJsonAsync("api/propertyapplications", form);
        return await ReadAsync<PropertyApplicationModel>(response);
    }

    public async Task<PropertyApplicationModel> UpdatePropertyApplicationAsync(int id, PropertyApplicationFormModel form)
    {
        var response = await httpClient.PutAsJsonAsync($"api/propertyapplications/{id}", form);
        return await ReadAsync<PropertyApplicationModel>(response);
    }

    public async Task DeletePropertyApplicationAsync(int id)
    {
        var response = await httpClient.DeleteAsync($"api/propertyapplications/{id}");
        await EnsureSuccessAsync(response);
    }

    private static string BuildQuery(string path, params (string Key, string? Value)[] values)
    {
        var queryParts = values
            .Where(item => !string.IsNullOrWhiteSpace(item.Value))
            .Select(item => $"{Uri.EscapeDataString(item.Key)}={Uri.EscapeDataString(item.Value!)}")
            .ToList();

        return queryParts.Count == 0 ? path : $"{path}?{string.Join("&", queryParts)}";
    }

    private static async Task<T> ReadAsync<T>(HttpResponseMessage response)
    {
        await EnsureSuccessAsync(response);
        return (await response.Content.ReadFromJsonAsync<T>())!;
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var message = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(message))
        {
            response.EnsureSuccessStatusCode();
        }

        throw new InvalidOperationException(message);
    }
}
