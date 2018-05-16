namespace DocumentBuilderservice
{
    using System.Collections.Generic;

    using PdfSharp.Pdf;

    public interface IPdfDocumentBuilder
	{
		PdfDocument BuildDocument(List<string> files);
		void SaveFile(PdfDocument document, string path);
	}
}