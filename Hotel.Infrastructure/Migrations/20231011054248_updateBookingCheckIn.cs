﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hotel.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateBookingCheckIn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "CheckInDate",
                table: "Bookings",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CheckInDate",
                table: "Bookings",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }
    }
}
