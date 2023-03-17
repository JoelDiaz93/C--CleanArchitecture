using CleanArchitecture.Data;
using CleanArchitecture.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

StreamerDbContext dbContext = new();

//await AddNewRecords();
//QueryStreaming();
//await QueryFilter();
//await QueryMethods();
//await QueryLinq();
//await TrackingAndNotTracking();
//await AddNewStreamerWithVideo();
//await AddNewStreamerWithVideoId();
//await AddNewActorWithVideo();
//await AddNewDirectorWithVideo();
await MultipleEntituesQuery();


Console.WriteLine("Presione cualquier tecla para terminar....");
Console.ReadKey();

async Task MultipleEntituesQuery()
{
    //var videoWithActores = await dbContext!.Videos!.Include(q => q.Actores).FirstOrDefaultAsync(q => q.Id == 1);

    //var actor = await dbContext!.Actores!.Select(q => q.Nombre).ToListAsync();
    //Consulta en varias tablas con la condicion que si no tienen director no se realiza la consulta
    var videoWithDirector = await dbContext!.Videos!
                            .Where(q => q.Director != null)
                            .Include(q => q.Director)
                            .Select(q =>
                                new
                                {
                                    Director_NombreCompleto = $"{q.Director.Nombre} {q.Director.Apellido}",
                                    Movie = q.Nombre
                                }
                             )
                            .ToListAsync(); 

    foreach(var pelicua in videoWithDirector)
    {
        Console.WriteLine($"{pelicua.Movie} - {pelicua.Director_NombreCompleto}");
    }
}

async Task AddNewDirectorWithVideo()
{
    var director = new Director
    {
        Nombre = "Maserati",
        Apellido = "Volenti",
        VideoId = 2,
    };

    await dbContext.AddAsync(director);
    await dbContext.SaveChangesAsync();
}
async Task AddNewActorWithVideo()
{
    var actor = new Actor
    {
        Nombre = "Vin",
        Apellido = "Diesel",
    };

    await dbContext.AddAsync(actor);
    await dbContext.SaveChangesAsync();

    var videoActor = new VideosActores
    {
        ActorId = actor.Id,
        VideoId = 5
    };

    await dbContext.AddAsync(videoActor);
    await dbContext.SaveChangesAsync();
}

async Task AddNewStreamerWithVideoId()
{
    var BatmanForever = new Video
    {
        Nombre = "Batman Forever",
        StreamerId = 1002,
    };

    await dbContext.AddAsync(BatmanForever);
    await dbContext.SaveChangesAsync();
}
async Task AddNewStreamerWithVideo()
{
    var pantallla = new Streamer
    {
        Nombre = "Pantalla"
    };

    var hunderGames = new Video
    {
        Nombre = "Hunger Games",
        Streamer = pantallla,
    };

    await dbContext.AddAsync(hunderGames);
    await dbContext.SaveChangesAsync();
}
async Task TrackingAndNotTracking()
{
    //Se mantiene en memoria disponible para realizar un update
    var streamerWithTracking = await dbContext!.Streamers!.FirstOrDefaultAsync(x => x.Id == 3);
    //No se mantiene en memoria por tanto no se va a realizar un update
    var streamerWithNotTracking = await dbContext!.Streamers!.AsNoTracking().FirstOrDefaultAsync(x => x.Id == 2);

    streamerWithTracking.Nombre = "Disney";
    streamerWithNotTracking.Nombre = "Amazon Premium";

    await dbContext.SaveChangesAsync();
}

async Task QueryLinq()
{
    Console.WriteLine($"Ingrese una compania de streaming: ");
    var streamingName = Console.ReadLine();
    var streamers = await (from i in dbContext.Streamers
                           where EF.Functions.Like(i.Nombre, $"%{streamingName}%")
                           select i).ToListAsync();

    foreach (var streamer in streamers)
    {
        Console.WriteLine($"{streamer.Id} - {streamer.Nombre}");
    }


}

async Task QueryMethods()
{
    var streamer = dbContext!.Streamers!;

    //Va asumir que existe la data y obtiene el primer registro del resultado, si no existe dispara un mensaje de error
    var firstAsync = await streamer.Where(y => y.Nombre.Contains("a")).FirstAsync();
    //Devuelve un valor por defecto null si no encuentra un registro
    var firstOrDefaultAsync_v1 = await streamer.Where(y => y.Nombre.Contains("a")).FirstOrDefaultAsync();
    //Se instacia directamente el FirstOrDefaultAsync
    var firstOrDefaultAsync_v2 = await streamer.FirstOrDefaultAsync(y => y.Nombre.Contains("a"));
    //El resultado debe ser un unico valor si tiene mas valores dispara una excepcion
    var singleAsync = await streamer.Where(y => y.Id == 1).SingleAsync();
    //Devuelve siempre un valor, si no existe un resultado devuelve un valor null pero no un error
    var singleOrDefaultAsync = await streamer.Where(y => y.Id == 1).SingleOrDefaultAsync();
    //Realiza busquedas de registros por su id, solo devuelve un resultado
    var resultado = await streamer.FindAsync(1);

    var count = await streamer.CountAsync();
    var longAcount = await streamer.LongCountAsync();
    var min = await streamer.MinAsync();
    var max = await streamer.MaxAsync();

}
async Task QueryFilter()
{
    Console.WriteLine($"Ingrese una compania de streaming: ");
    var streamingName = Console.ReadLine();
    //var streamers = await dbContext!.Streamers!.Where(x => x.Nombre == streamingName).ToListAsync();
    var streamers = await dbContext!.Streamers!.Where(x => x.Nombre.Equals(streamingName)).ToListAsync();

    foreach (var streamer in streamers)
    {
        Console.WriteLine($"{streamer.Id} - {streamer.Nombre}");
    }

    //var streamerPartialResults = await dbContext!.Streamers!.Where(x => x.Nombre.Contains(streamingName)).ToListAsync();
    var streamerPartialResults = await dbContext!.Streamers!.Where(x => EF.Functions.Like(x.Nombre, $"%{streamingName}%")).ToListAsync();
    foreach (var streamer in streamerPartialResults)
    {
        Console.WriteLine($"{streamer.Id} - {streamer.Nombre}");
    }
}

void QueryStreaming()
{
    var streamers = dbContext!.Streamers!.ToList();

    foreach(var streamer in streamers)
    {
        Console.WriteLine($"{streamer.Id} - {streamer.Nombre}");
    }
}

async Task AddNewRecords()
{
    Streamer streamer = new()
    {
        Nombre = "Disney plus",
        Url = "http://www.disneyplus.com"
    };

    dbContext!.Streamers!.Add(streamer);

    await dbContext.SaveChangesAsync();

    var movies = new List<Video>
    {
        new Video
        {
            Nombre = "Cenicienta",
            StreamerId = streamer.Id,
        },
        new Video
        {
            Nombre = "Avengers End Game",
            StreamerId = streamer.Id,
        },
        new Video
        {
            Nombre = "El planeta del tesoro",
            StreamerId = streamer.Id,
        },
        new Video
        {
            Nombre = "Star Wars 1",
            StreamerId = streamer.Id,
        },
        new Video
        {
            Nombre = "Avatar 1",
            StreamerId = streamer.Id,
        }
    };

    await dbContext!.AddRangeAsync(movies);
    await dbContext!.SaveChangesAsync();
}