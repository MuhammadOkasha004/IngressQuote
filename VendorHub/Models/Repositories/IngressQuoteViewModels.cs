using System;
using System.Collections.Generic;

namespace VendorHub.Models.Repositories
{
    public class QuotationComparisonViewModel
    {
        public int QuotationId { get; set; }
        public string VendorName { get; set; } = null!;
        public string VendorCompanyName { get; set; } = null!;
        public decimal QuotationAmount { get; set; }
        public string Status { get; set; } = null!;
    }

    public class SimpleQuotationSummaryViewModel
    {
        public int QuotationRequestId { get; set; }
        public string RequestTitle { get; set; } = null!;
        public DateTime RequestDate { get; set; }
        public string QuotationTitle { get; set; } = null!;
        public string QuotationStatus { get; set; } = null!;
        public decimal QuotationAmount { get; set; }
    }

    public class LowestQuotationViewModel
    {
        public int QuotationId { get; set; }
        public string RequestTitle { get; set; } = null!;
        public string VendorName { get; set; } = null!;
        public string VendorCompanyName { get; set; } = null!;
        public decimal QuotationAmount { get; set; }
        public string Status { get; set; } = null!;
    }

    public class TrackQuotationViewModel
    {
        public int QuotationId { get; set; }
        public string RequestTitle { get; set; } = null!;
        public string VendorName { get; set; } = null!;
        public string Status { get; set; } = null!;
        public decimal QuotationAmount { get; set; }
    }

    public class TopVendorStat
    {
        public string VendorName { get; set; } = "";
        public int ResponseCount { get; set; }
    }

    public class AdminDashboardStats
    {
        public int TotalVendors { get; set; }
        public int ActiveQuotations { get; set; }
        public int PendingQuotations { get; set; }
        public int ApprovedQuotations { get; set; }
    }

    public class VendorDashboardStats
    {
        public int TotalRequestsSent { get; set; }
        public int TotalRequestsReceived { get; set; }
        public int SubmittedQuotes { get; set; }
        public int PendingApproval { get; set; }
        public int WonOrders { get; set; }
        public int PendingRequests { get; set; }
        public int SubmittedResponses { get; set; }
        public int RejectedResponses { get; set; }
        public decimal AvgBidPrice { get; set; }
        public decimal AvgBudget { get; set; }
    }

    public class LoginResponse
    {
        public string Token { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int UserId { get; set; }
        public int CompanyId { get; set; }
    }

    public class VendorResponseDto
    {
        public int QuotationAssignmentId { get; set; }
        public int VendorId { get; set; }
        public decimal Price { get; set; }
        public string? Notes { get; set; }
        public int DeliveryTimeDays { get; set; }
    }

    public class CreateQuotationRequestDto
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime? Deadline { get; set; }
        public decimal? Budget { get; set; }
        public int CompanyId { get; set; }
        public List<int>? VendorIds { get; set; }
    }
}
