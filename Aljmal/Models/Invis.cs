using Aljmal.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;
public class InvoiceDocument : IDocument
{
    private readonly OrderToFileDto _model;

    public InvoiceDocument(OrderToFileDto model)
    {
        _model = model;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(2, Unit.Centimetre);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontSize(14));
            //page.Content().Decoration();

            // Header with image
            page.Header()
                .Column(column =>
                {
                    // Add the image at the top
                    //if (System.IO.File.Exists(_imagePath))
                    //{
                    //    column.Item()
                    //        .AlignCenter()
                    //        .Image(_imagePath, ImageScaling.FitArea);
                    //}

                    // Add the invoice title below the image
                    column.Item()
                        .AlignCenter()
                        .PaddingTop(10)
                        .Text("فاتورة")
                        .SemiBold().FontSize(24).FontFamily("Arial");
                });

            // Rest of the content remains the same
            page.Content()
                .PaddingVertical(1, Unit.Centimetre)
                .Column(x =>
                {
                    x.Item().Text($"طلب: {_model.CustomerName}");
                    x.Item().Text($"تاريخ: {_model.OrderDate:dd/MM/yyyy}");

                    x.Item().PaddingTop(15).Table(table =>
                    {
                        // Table configuration as before
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(25);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("رقم").SemiBold();
                            header.Cell().Text("اسم المنتج").SemiBold();
                            header.Cell().Text("سعر الوحدة").SemiBold();
                            header.Cell().Text("الكمية").SemiBold();
                            header.Cell().Text("المجموع").SemiBold();
                        });
                        int n = 1;
                        foreach (var item in _model.getOrderInfos)
                        {
                            table.Cell().Text(n.ToString());
                            table.Cell().Text(item.ProductName);
                            table.Cell().Text($"{item.UnitPrice} ريال");
                            table.Cell().Text(item.Count.ToString());
                            table.Cell().Text($"{item.UnitPrice*item.Count} ريال");
                            n++;
                        }
                        table.Footer(footer =>
                        {
                            footer.Cell().ColumnSpan(3).Text("المجموع:").Bold();
                            footer.Cell().Text($"{_model.TotalPrice} ريال").Bold();
                        });
                    });

                    //x.Item().AlignRight().PaddingTop(10)
                    //    .Text($"المجموع: ").SemiBold();
                });
        });
    }
}

public class OrderToFileDto
{
    public DateTime? OrderDate { get; set; }
    public string? CustomerName { get; set; }
    public string? SellerName { get; set; }
    public decimal? TotalPrice { get; set; }
    public List<GetOrderInfo>? getOrderInfos { get; set; }
}

//public class InvoiceItem
//{
//    public int Index { get; set; }
//    public string ProductName { get; set; }
//    public decimal UnitPrice { get; set; }
//    public int Quantity { get; set; }
//    public decimal Total { get; set; }
//}


//public IActionResult DownloadInvoice()
//{
//    // Create sample data
//    var model = new InvoiceModel
//    {
//        CustomerName = "خالد شيخ",
//        Date = new DateTime(2025, 5, 24),
//        Items = new List<InvoiceItem>
//        {
//            new InvoiceItem { Index = 1, ProductName = "الفاتق", UnitPrice = 40, Quantity = 5, Total = 200 },
//            new InvoiceItem { Index = 2, ProductName = "الفاتق", UnitPrice = 40, Quantity = 5, Total = 200 }
//        },
//        Total = 8000
//    };

//    // Get the path to the image file
//    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logo.png");

//    // Generate PDF with image
//    var document = new InvoiceDocument(model, imagePath);
//    var stream = new MemoryStream();
//    document.GeneratePdf(stream);
//    stream.Position = 0;

//    return File(stream, "application/pdf", "Invoice.pdf");
//}