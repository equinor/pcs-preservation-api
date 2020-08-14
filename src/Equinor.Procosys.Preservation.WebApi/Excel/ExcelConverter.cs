﻿using System;
using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Query.GetTagsQueries.GetTagsForExport;

namespace Equinor.Procosys.Preservation.WebApi.Excel
{
    public class ExcelConverter : IExcelConverter
    {
        private readonly IPlantProvider _plantProvider;

        public ExcelConverter(IPlantProvider plantProvider) => _plantProvider = plantProvider;

        public MemoryStream Convert(ExportDto dto)
        {
            var excelStream = new MemoryStream();

            using (var workbook = new XLWorkbook())
            {
                CreateFrontSheet(workbook, dto.UsedFilter);
                CreateTagSheet(workbook, dto.Tags);

                workbook.SaveAs(excelStream);
            }

            return excelStream;
        }

        private void CreateTagSheet(XLWorkbook workbook, IEnumerable<ExportTagDto> tags)
        {
            var colIdx = 0;

            var tagNoCol = ++colIdx;
            var descriptionCol = ++colIdx;
            var nextCol = ++colIdx;
            var dueCol = ++colIdx;
            var modeCol = ++colIdx;
            var poCol = ++colIdx;
            var areaCol = ++colIdx;
            var respCol = ++colIdx;
            var discCol = ++colIdx;
            var statusCol = ++colIdx;
            var reqTypeCol = ++colIdx;
            var actionCol = ++colIdx;

            var sheet = workbook.Worksheets.Add("Tags");

            var rowIdx = 0;
            var row = sheet.Row(++rowIdx);
            row.Style.Font.SetBold();
            row.Style.Font.SetFontSize(12);
            row.Cell(tagNoCol).Value = "Tag nr";
            row.Cell(descriptionCol).Value = "Description";
            row.Cell(nextCol).Value = "Next";
            row.Cell(dueCol).Value = "Due";
            row.Cell(modeCol).Value = "Mode";
            row.Cell(poCol).Value = "Purchase order";
            row.Cell(areaCol).Value = "Area";
            row.Cell(respCol).Value = "Responsible";
            row.Cell(discCol).Value = "Discipline";
            row.Cell(statusCol).Value = "Status";
            row.Cell(reqTypeCol).Value = "Req. type";
            row.Cell(actionCol).Value = "Action status";

            foreach (var tag in tags)
            {
                row = sheet.Row(++rowIdx);

                row.Cell(tagNoCol).Value = tag.TagNo;
                row.Cell(descriptionCol).Value = tag.Description;
                row.Cell(nextCol).Value = "TODO"; // TODO EXPAND DTO W NEXT
                row.Cell(dueCol).Value = "TODO";
                row.Cell(modeCol).Value = tag.Mode;
                row.Cell(poCol).Value = tag.PurchaseOrderTitle;
                row.Cell(areaCol).Value = tag.AreaCode;
                row.Cell(respCol).Value = tag.ResponsibleCode;
                row.Cell(discCol).Value = tag.DisciplineCode;
                row.Cell(statusCol).Value = tag.Status;
                row.Cell(reqTypeCol).Value = tag.RequirementTitles;
                row.Cell(actionCol).Value = tag.ActionStatus;
            }
        }

        private void CreateFrontSheet(XLWorkbook workbook, UsedFilterDto usedFilter)
        {
            var sheet = workbook.Worksheets.Add("Filters");
            var rowIdx = 0;
            var row = sheet.Row(++rowIdx);
            row.Style.Font.SetBold();
            row.Style.Font.SetFontSize(12);
            row.Cell(1).Value = $"Export of preserved tags from project {usedFilter.ProjectName} in plant {_plantProvider.Plant}";

            sheet.Row(++rowIdx).Cell(1).Value = "Used filter values:";
            AddUsedFilter(sheet.Row(++rowIdx), "Tag number starts with", usedFilter.TagNoStartsWith);
            AddUsedFilter(sheet.Row(++rowIdx), "Purchase order number starts with", usedFilter.PurchaseOrderNoStartsWith);
            AddUsedFilter(sheet.Row(++rowIdx), "Calloff number starts with", usedFilter.CallOffStartsWith);
            AddUsedFilter(sheet.Row(++rowIdx), "CommPkg number starts with", usedFilter.CommPkgNoStartsWith);
            AddUsedFilter(sheet.Row(++rowIdx), "McPkg number starts with", usedFilter.McPkgNoStartsWith);
            AddUsedFilter(sheet.Row(++rowIdx), "Storage area starts with", usedFilter.StorageAreaStartsWith);
            AddUsedFilter(sheet.Row(++rowIdx), "Preservation status", usedFilter.PreservationStatus);
            AddUsedFilter(sheet.Row(++rowIdx), "Preservation actions", usedFilter.ActionStatus);
            AddUsedFilter(sheet.Row(++rowIdx), "Voided/unvoided tags", usedFilter.VoidedFilter);
            AddUsedFilter(sheet.Row(++rowIdx), "Preservation dute date", usedFilter.DueFilters);
            AddUsedFilter(sheet.Row(++rowIdx), "Journeys", usedFilter.JourneyTitles);
            AddUsedFilter(sheet.Row(++rowIdx), "Steps", usedFilter.StepTitles);
            AddUsedFilter(sheet.Row(++rowIdx), "Modes", usedFilter.ModeTitles);
            AddUsedFilter(sheet.Row(++rowIdx), "Requirements", usedFilter.RequirementTypeTitles);
            AddUsedFilter(sheet.Row(++rowIdx), "Requirements", usedFilter.RequirementTypeTitles);
            AddUsedFilter(sheet.Row(++rowIdx), "Tag functions", usedFilter.TagFunctionCodes);
            AddUsedFilter(sheet.Row(++rowIdx), "Disciplines", usedFilter.DisciplineCodes);
            AddUsedFilter(sheet.Row(++rowIdx), "Responsibles", usedFilter.ResponsibleCodes);
            AddUsedFilter(sheet.Row(++rowIdx), "Areas", usedFilter.AreaCodes);
        }

        private void AddUsedFilter(IXLRow row, string label, IEnumerable<string> values)
            => AddUsedFilter(row, label, string.Join(",", values));

        private void AddUsedFilter(IXLRow row, string label, string value)
        {
            row.Cell(1).Value = label;
            row.Cell(2).Value = value;
        }

        public string GetFileName()=> $"PreservedTags-{DateTime.Now:yyyyMMdd-hhmmss}";
    }
}
