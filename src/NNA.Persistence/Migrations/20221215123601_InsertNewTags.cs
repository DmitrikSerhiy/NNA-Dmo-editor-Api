using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class InsertNewTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string bathosSql =
                @$" insert into [dbo].[Tags]
                  (id, Name, [Description], DateOfCreation)
                  VALUES (
                    '{Guid.NewGuid()}',
                    'Bathos',
                    'Bathos refers to rhetorical anticlimax, an abrupt transition from a lofty style or grand topic to a common or vulgar one, occurring either accidentally (through artistic ineptitude) or intentionally (for comic effect).',
                    {DateTimeOffset.UtcNow.UtcTicks});";
            
            string clicheSql =
                @$" insert into [dbo].[Tags]
                  (id, Name, [Description], DateOfCreation)
                  VALUES (
                    '{Guid.NewGuid()}',
                    'Cliché',
                    'A cliché is an element of an artistic work, saying, or idea that has become overused to the point of losing its original meaning or effect, even to the point of being weird or irritating, especially when at some earlier time it was considered meaningful',
                    {DateTimeOffset.UtcNow.UtcTicks});";
            
            string wilhelmScreamSql =
                @$" insert into [dbo].[Tags]
                  (id, Name, [Description], DateOfCreation)
                  VALUES (
                    '{Guid.NewGuid()}',
                    'Wilhelm scream',
                    'The Wilhelm scream is a stock sound effect that has been used in a number of films and TV series. The scream is usually used when someone is shot, falls from a great height, or is thrown from an explosion.',
                    {DateTimeOffset.UtcNow.UtcTicks});";
            
            string intertextSql =
                @$" insert into [dbo].[Tags]
                  (id, Name, [Description], DateOfCreation)
                  VALUES (
                    '{Guid.NewGuid()}',
                    'Intertext',
                    'Intertextuality is the shaping of a texts meaning by another text, either through deliberate compositional strategies such as allusion, calque, plagiarism, translation, refference, pastiche or parody or by interconnections between similar or related works perceived by an audience or reader of the text.',
                    {DateTimeOffset.UtcNow.UtcTicks});";
            
            string quotationSql =
                @$" insert into [dbo].[Tags]
                  (id, Name, [Description], DateOfCreation)
                  VALUES (
                    '{Guid.NewGuid()}',
                    'Quotation',
                    'A quotation is the repetition of a sentence, phrase, or passage from speech or text that someone has said or written.',
                    {DateTimeOffset.UtcNow.UtcTicks});";
            
            migrationBuilder.Sql(bathosSql);
            migrationBuilder.Sql(clicheSql);
            migrationBuilder.Sql(wilhelmScreamSql);
            migrationBuilder.Sql(intertextSql);
            migrationBuilder.Sql(quotationSql);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
