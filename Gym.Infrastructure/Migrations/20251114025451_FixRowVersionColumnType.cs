using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gym.Infrastructure.Migrations
{
    public partial class FixRowVersionColumnType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Kiểm tra cột RowVersion có tồn tại hay không trước khi xóa
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                           WHERE TABLE_NAME = 'Members' 
                           AND COLUMN_NAME = 'RowVersion')
                BEGIN
                    -- Drop the old RowVersion column
                    ALTER TABLE Members DROP COLUMN RowVersion;
                END");

            // Thêm cột RowVersion mới với kiểu dữ liệu rowversion
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Members",
                type: "rowversion",  // Đảm bảo kiểu dữ liệu là rowversion
                rowVersion: true,    // Đảm bảo rằng nó được sử dụng để kiểm soát đồng thời
                nullable: false);    // Cột không được phép null
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Xóa cột RowVersion khi rollback
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Members");
        }
    }
}