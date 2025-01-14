using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace L2X.Exchange.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Member",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: true),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Phone = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Password = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Passcode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Affiliate = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                    Secure = table.Column<string>(type: "text", nullable: true),
                    Level = table.Column<byte>(type: "smallint", nullable: true),
                    Rank = table.Column<byte>(type: "smallint", nullable: true),
                    ValidRate = table.Column<byte>(type: "smallint", nullable: true),
                    LogFailed = table.Column<byte>(type: "smallint", nullable: true),
                    LockedTo = table.Column<long>(type: "bigint", nullable: true),
                    LastLogin = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedAt = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Member", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ticker",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    ChainId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Fullname = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    Site = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Logo = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Summary = table.Column<string>(type: "text", nullable: true),
                    Options = table.Column<string>(type: "character varying(1600)", maxLength: 1600, nullable: true),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Usage = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    BaseFactor = table.Column<long>(type: "bigint", nullable: true),
                    Precision = table.Column<short>(type: "smallint", nullable: true),
                    SubUnits = table.Column<short>(type: "smallint", nullable: true),
                    Price = table.Column<decimal>(type: "numeric", nullable: true),
                    MinCollect = table.Column<decimal>(type: "numeric", nullable: true),
                    MinDeposit = table.Column<decimal>(type: "numeric", nullable: true),
                    MinWithdraw = table.Column<decimal>(type: "numeric", nullable: true),
                    Position = table.Column<int>(type: "integer", nullable: true),
                    Creator = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Modifier = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    CreatedAt = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedAt = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ticker", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ticker_Ticker_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Ticker",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: true),
                    TickerId = table.Column<Guid>(type: "uuid", nullable: true),
                    Balance = table.Column<decimal>(type: "numeric", nullable: true),
                    LockAmount = table.Column<decimal>(type: "numeric", nullable: true),
                    CreatedAt = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedAt = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Account_Member_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Account_Ticker_TickerId",
                        column: x => x.TickerId,
                        principalTable: "Ticker",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Market",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EngineId = table.Column<Guid>(type: "uuid", nullable: true),
                    BaseId = table.Column<Guid>(type: "uuid", nullable: true),
                    QuoteId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Fullname = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    Data = table.Column<string>(type: "character varying(1600)", maxLength: 1600, nullable: true),
                    VolumePrecision = table.Column<short>(type: "smallint", nullable: true),
                    PricePrecision = table.Column<short>(type: "smallint", nullable: true),
                    MinVolumn = table.Column<decimal>(type: "numeric", nullable: true),
                    MinPrice = table.Column<decimal>(type: "numeric", nullable: true),
                    MaxPrice = table.Column<decimal>(type: "numeric", nullable: true),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    State = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Position = table.Column<int>(type: "integer", nullable: true),
                    Creator = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Modifier = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    CreatedAt = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedAt = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Market", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Market_Ticker_BaseId",
                        column: x => x.BaseId,
                        principalTable: "Ticker",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Market_Ticker_QuoteId",
                        column: x => x.QuoteId,
                        principalTable: "Ticker",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    MarketId = table.Column<Guid>(type: "uuid", nullable: false),
                    RuleId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrderNo = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Symbol = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Side = table.Column<bool>(type: "boolean", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: true),
                    Condition = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Price = table.Column<decimal>(type: "numeric", nullable: true),
                    StopPrice = table.Column<decimal>(type: "numeric", nullable: true),
                    Volume = table.Column<decimal>(type: "numeric", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: true),
                    Origin = table.Column<decimal>(type: "numeric", nullable: true),
                    Locked = table.Column<decimal>(type: "numeric", nullable: true),
                    Funded = table.Column<decimal>(type: "numeric", nullable: true),
                    Matched = table.Column<int>(type: "integer", nullable: true),
                    State = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreatedAt = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedAt = table.Column<long>(type: "bigint", nullable: true),
                    ExpiredAt = table.Column<long>(type: "bigint", nullable: true),
                    FinishedAt = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Order_Market_MarketId",
                        column: x => x.MarketId,
                        principalTable: "Market",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Order_Member_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PreOrder",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    MarketId = table.Column<Guid>(type: "uuid", nullable: false),
                    Symbol = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Side = table.Column<bool>(type: "boolean", maxLength: 20, nullable: false),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Condition = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Price = table.Column<decimal>(type: "numeric", nullable: true),
                    StopPrice = table.Column<decimal>(type: "numeric", nullable: true),
                    Volume = table.Column<decimal>(type: "numeric", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: true),
                    Origin = table.Column<decimal>(type: "numeric", nullable: true),
                    CreatedAt = table.Column<long>(type: "bigint", nullable: false),
                    ExpiredAt = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PreOrder_Market_MarketId",
                        column: x => x.MarketId,
                        principalTable: "Market",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PreOrder_Member_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Match",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MakerId = table.Column<Guid>(type: "uuid", nullable: false),
                    TakerId = table.Column<Guid>(type: "uuid", nullable: false),
                    MarketId = table.Column<Guid>(type: "uuid", nullable: false),
                    MkrOrdId = table.Column<Guid>(type: "uuid", nullable: false),
                    TkrOrdId = table.Column<Guid>(type: "uuid", nullable: false),
                    TakerType = table.Column<bool>(type: "boolean", maxLength: 20, nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: true),
                    Volume = table.Column<decimal>(type: "numeric", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: true),
                    MakerFee = table.Column<decimal>(type: "numeric", nullable: true),
                    TakerFee = table.Column<decimal>(type: "numeric", nullable: true),
                    CreatedAt = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Match", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Match_Market_MarketId",
                        column: x => x.MarketId,
                        principalTable: "Market",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Match_Member_MakerId",
                        column: x => x.MakerId,
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Match_Member_TakerId",
                        column: x => x.TakerId,
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Match_Order_MkrOrdId",
                        column: x => x.MkrOrdId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Match_Order_TkrOrdId",
                        column: x => x.TkrOrdId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_OwnerId_TickerId",
                table: "Account",
                columns: new[] { "OwnerId", "TickerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Account_TickerId",
                table: "Account",
                column: "TickerId");

            migrationBuilder.CreateIndex(
                name: "IX_Market_BaseId",
                table: "Market",
                column: "BaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Market_Name",
                table: "Market",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Market_QuoteId",
                table: "Market",
                column: "QuoteId");

            migrationBuilder.CreateIndex(
                name: "IX_Match_MakerId",
                table: "Match",
                column: "MakerId");

            migrationBuilder.CreateIndex(
                name: "IX_Match_MarketId",
                table: "Match",
                column: "MarketId");

            migrationBuilder.CreateIndex(
                name: "IX_Match_MkrOrdId",
                table: "Match",
                column: "MkrOrdId");

            migrationBuilder.CreateIndex(
                name: "IX_Match_TakerId",
                table: "Match",
                column: "TakerId");

            migrationBuilder.CreateIndex(
                name: "IX_Match_TkrOrdId",
                table: "Match",
                column: "TkrOrdId");

            migrationBuilder.CreateIndex(
                name: "IX_Member_Email",
                table: "Member",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Member_Name",
                table: "Member",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Member_Phone",
                table: "Member",
                column: "Phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_MarketId",
                table: "Order",
                column: "MarketId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_OrderNo",
                table: "Order",
                column: "OrderNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_OwnerId",
                table: "Order",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_PreOrder_MarketId",
                table: "PreOrder",
                column: "MarketId");

            migrationBuilder.CreateIndex(
                name: "IX_PreOrder_OwnerId",
                table: "PreOrder",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticker_Name",
                table: "Ticker",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ticker_ParentId",
                table: "Ticker",
                column: "ParentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "Match");

            migrationBuilder.DropTable(
                name: "PreOrder");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "Market");

            migrationBuilder.DropTable(
                name: "Member");

            migrationBuilder.DropTable(
                name: "Ticker");
        }
    }
}
