using FluentValidation;
using PostalSystem.Api.Dtos;

public class TransferRequestValidator : AbstractValidator<TransferRequest>
{
    public TransferRequestValidator()
    {
        RuleFor(x => x.TrackingGuid).NotEmpty();
        RuleFor(x => x.PostmanId).GreaterThan(0);
    }

    // There's something magical about coding whilst listening to 80's musics such as
    // "Hal & Oat's Out Of Touch Thursdays", anime BGMs like the ones from KS or BA, 
    // and Bardcore versions of famous songs like "Rick Astley's Never Gonna Give
    // You Up!" or "All Star!" (Somebody Once Told Me) that I cannot describe!
    // It doesn't help with the actual coding at all, and it certainly increases the
    // needed time to finish tasks at hand but it helps me to be patient with the coding
    // process which is just what I need to remain sane sometimes XD
    // Although I cannot deny the fact that AI has been a huge help in this- hopefully 
    // fruitful -coding journey of mine! Without it I may have not even made it this far!
    // Sometimes I kinda wish I was actually as smart as the people who actually did
    // these things before AI. No wonder most used to go bald whilst still in their 
    // twenties or early thirties! OK I better get back to work before someone catches
    // me writing this super long comment that serves no purpose whatsoever XD
}