using ProToolRent.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProToolRent.Application.DTOs
{
    public record ToolDto
    (
        Guid Id,
        string Brand,
        string Name,
        double Power,
        string Description,
        string Status,
        double Price,
        Guid CategoryId,
        Guid UserId
    )
    {
        public static ToolDto FromEntity(Tool tool)
        {
            return new ToolDto(
                tool.Id,
                tool.Specification.Brand,
                tool.Specification.Name,
                tool.Specification.Power,
                tool.Description,
                tool.Status,
                tool.Price,
                tool.CategoryId,
                tool.UserId);
        }
    }
}

