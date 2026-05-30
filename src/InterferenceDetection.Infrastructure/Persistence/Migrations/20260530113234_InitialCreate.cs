using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterferenceDetection.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cells",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    latitude = table.Column<double>(type: "double precision", nullable: false),
                    longitude = table.Column<double>(type: "double precision", nullable: false),
                    coverage_radius_meters = table.Column<double>(type: "double precision", nullable: false),
                    transmit_power_dbm = table.Column<double>(type: "double precision", nullable: false),
                    technology = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    nrarfcn = table.Column<int>(type: "integer", nullable: true),
                    physical_cell_id = table.Column<int>(type: "integer", nullable: true),
                    gnb_id = table.Column<long>(type: "bigint", nullable: true),
                    tracking_area_code = table.Column<int>(type: "integer", nullable: true),
                    earfcn = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cells", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cells");
        }
    }
}
