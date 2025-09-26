using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeoSense.API.Migrations
{
    /// <inheritdoc />
    public partial class InitalAzureSQLSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PATIO",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NOME = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PATIO", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "USUARIO",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NOME = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EMAIL = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SENHA = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TIPO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USUARIO", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "VAGA",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NUMERO = table.Column<int>(type: "int", nullable: false),
                    TIPO = table.Column<int>(type: "int", nullable: false),
                    STATUS = table.Column<int>(type: "int", nullable: false),
                    PATIO_ID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VAGA", x => x.ID);
                    table.ForeignKey(
                        name: "FK_VAGA_PATIO_PATIO_ID",
                        column: x => x.PATIO_ID,
                        principalTable: "PATIO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MOTO",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MODELO = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PLACA = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CHASSI = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PROBLEMA_IDENTIFICADO = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    VAGA_ID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MOTO", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MOTO_VAGA_VAGA_ID",
                        column: x => x.VAGA_ID,
                        principalTable: "VAGA",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MOTO_CHASSI",
                table: "MOTO",
                column: "CHASSI",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MOTO_PLACA",
                table: "MOTO",
                column: "PLACA",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MOTO_VAGA_ID",
                table: "MOTO",
                column: "VAGA_ID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_USUARIO_EMAIL",
                table: "USUARIO",
                column: "EMAIL",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VAGA_NUMERO_PATIO_ID",
                table: "VAGA",
                columns: new[] { "NUMERO", "PATIO_ID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VAGA_PATIO_ID",
                table: "VAGA",
                column: "PATIO_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MOTO");

            migrationBuilder.DropTable(
                name: "USUARIO");

            migrationBuilder.DropTable(
                name: "VAGA");

            migrationBuilder.DropTable(
                name: "PATIO");
        }
    }
}
