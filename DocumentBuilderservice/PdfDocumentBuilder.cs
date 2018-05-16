﻿namespace DocumentBuilderservice
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;

    using MigraDoc.DocumentObjectModel;
    using MigraDoc.Rendering;

    using NLog;

    using PdfSharp.Pdf;
	using CodeRewriting;

	public class PdfDocumentBuilder : IPdfDocumentBuilder
	{
        private Logger _logger;

		public PdfDocument BuildDocument(List<string> files)
        {
            _logger = LogManager.GetCurrentClassLogger();
            var document = new Document();
            var section = document.AddSection();
            for (int i = 0; i < files.Count(); i++)
            {
                if (TryOpen(files[i], 3))
                {
                    var img = section.AddImage(files[i]);
                    img.LockAspectRatio = true;
                    img.Left = -70;
                    if (img.Height > img.Width)
                    {
                        img.Height = document.DefaultPageSetup.PageHeight;
                    }
                    else
                    { 
                        img.Width = document.DefaultPageSetup.PageWidth;
                    }

                    if (i < files.Count() - 1)
                    { 
                        section.AddPageBreak();
                    }
                }
                else
                {
                    files.Remove(files[i]);
                }
            }

            var render = new PdfDocumentRenderer();
            render.Document = document;
            try
            {
                render.RenderDocument();
            }
            catch (Exception e)
            {
                _logger.Error(e.ToString());

                throw;
            }

            return render.PdfDocument;
        }

		public void SaveFile(PdfDocument document, string path)
        {
            document.Save(path);
            Logger logger = LogManager.GetCurrentClassLogger();
            logger.Trace("Document saved to " + path);
        }

        private bool TryOpen(string fileName, int tryCount)
        {
            for (int i = 0; i < tryCount; i++)
            {
                try
                {
                    var file = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
                    file.Close();

                    return true;
                }
                catch (IOException e)
                {
                    Thread.Sleep(1000);
                    _logger.Error(e.ToString());
                }
            }

            return false;
        }
    }
}
