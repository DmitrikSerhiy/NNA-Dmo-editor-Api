namespace NNA.Api.Helpers;

public static class ValidatorHelpers {
    // regex html5 standard. Took from https://html.spec.whatwg.org/multipage/input.html#valid-e-mail-address
    // it fit angular built in validation
    public static string EmailRegex =>
        @"^[a-zA-Z0-9.!#$%&'*+\/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$";
}