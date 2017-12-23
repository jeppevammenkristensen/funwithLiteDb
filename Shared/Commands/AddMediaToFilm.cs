using MediatR;

namespace Shared.Commands
{
    public class AddMediaToFilm : IRequest
    {
        public AddMediaToFilm(int filmId, TypeofMedia typeOfMedia, int amount)
        {
            FilmId = filmId;
            TypeOfMedia = typeOfMedia;
            Amount = amount;
        }

        public int FilmId { get; }
        public int Amount { get;  }
        public TypeofMedia TypeOfMedia { get; }
    }
}