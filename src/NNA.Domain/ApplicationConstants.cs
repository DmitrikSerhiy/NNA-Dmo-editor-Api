namespace NNA.Domain;

public static class ApplicationConstants {
    public const short MaxCollectionNameLength = 60;
    public const short MaxMovieTitleLength = 100;
    
    public const short MaxEntityNameLength = 100;
    public const short MaxEntityNameLongLength = 500;
    
    

    public const short MaxUserEmailLength = 50;

    public const short MaxUserNameLength = 50;
    public const short MaxPasswordLength = 30;
    public const short MinPasswordLength = 10;

    public const short MaxCharacterNameLength = 60;
    public const short MaxCharacterAliasesLength = 100;
    
    
    
    public const string NnaCharacterInterpolatorPrefix = "{{nna-character-";
    public const string NnaCharacterInterpolatorPostfix = "-nna-character}}";

    public const string NotActiveUserPolicy = "NotActiveUser";
    public const string ActiveUserPolicy = "ActiveUser";
    public const string SuperUserPolicy = "SuperUser";
}