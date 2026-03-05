using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Medical_Records_Manager.Migrations
{
    /// <inheritdoc />
    public partial class AddGiftCardToPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GiftCardId",
                table: "Payments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_GiftCardId",
                table: "Payments",
                column: "GiftCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_GiftCards_GiftCardId",
                table: "Payments",
                column: "GiftCardId",
                principalTable: "GiftCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_GiftCards_GiftCardId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_GiftCardId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "GiftCardId",
                table: "Payments");
        }
    }
}
