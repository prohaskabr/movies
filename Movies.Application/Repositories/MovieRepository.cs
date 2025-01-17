using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class MovieRepository(IDbConnectionFactory connectionFactory) : IMovieRepository
{

    public async Task<bool> CreateAsync(Movie movie, CancellationToken token = default)
    {
        using var conn = await connectionFactory.CreateConnectionAsync(token);
        using var trans = conn.BeginTransaction();

        var result = await conn.ExecuteAsync(new CommandDefinition(""""
            insert into movies (id, slug, title, yearofrelease)
            values (@id, @Slug, @Title, @YearOfRelease)
            """", movie, cancellationToken: token));

        if (result > 0)
        {
            foreach (var item in movie.Genres)
            {
                await conn.ExecuteAsync(new CommandDefinition(""""
                    insert into genres (movieId, name)
                    values (@MovieId, @Name)
                    """", new { MovieId = movie.Id, Name = item }, cancellationToken: token));
            }
        }

        trans.Commit();

        return result > 0;
    }

    public async Task<Movie?> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken token = default)
    {
        using var conn = await connectionFactory.CreateConnectionAsync(token);

        var movie = await conn.QuerySingleOrDefaultAsync<Movie>(
             new CommandDefinition("""
            select
                m.*, round(avg(r.rating), 1) as rating, myr.rating as userrating 
            from
                movies m
                left join ratings r on m.id = r.movieid
                left join ratings myr on m.id = myr.movieid and myr.userid = @userId
            where
                id = @id
            group by id, userrating
            """, new { id, userId }, cancellationToken: token));

        if (movie is null)
            return null;

        var genres = await conn.QueryAsync<string>(
            new CommandDefinition(""""
            select name from genres
            where movieId = @Id            
            """", new { movie.Id }, cancellationToken: token));

        foreach (var item in genres)
        {
            movie.Genres.Add(item);
        }

        return movie;
    }

    public async Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOptions options, CancellationToken token = default)
    {
        var orderClause = string.Empty;
        if (options.SortField is not null)
        {
            orderClause =  $"""
                , m.{options.SortField}
                order by m.{options.SortField} {(options.SortOrder == SortOrder.Ascending ? "asc" : "desc")}
                """;
        }

        using var connection = await connectionFactory.CreateConnectionAsync(token);
        var result = await connection.QueryAsync(
            new CommandDefinition($"""
            select
                m.*,
                string_agg(distinct g.name, ',') as genres,
                round(avg(r.rating), 1) as rating,
                myr.rating as userrating 
            from
                movies m
                left join genres g on m.id = g.movieid
                left join ratings r on m.id = r.movieid
                left join ratings myr on m.id = myr.movieid and myr.userid = @userId                
            where
                (@title is null or m.title like ('%' || @title || '%'))
                and (@year is null or m.yearofrelease = @year) 
            group by id, userrating {orderClause}
            limit  @pageSize
            offset @pageOffset
            """, new { 
                userId = options.UserId,
                title = options.Title,
                year = options.Year,
                pageSize = options.PageSize,
                pageOffset = (options.Page - 1 ) * options.PageSize,
            
            }, cancellationToken: token));

        return result.Select(x => new Movie
        {
            Id = x.id,
            Title = x.title,
            YearOfRelease = x.yearofrelease,
            Rating = (float?)x.rating,
            UserRating = (int?)x.userrating,
            Genres = Enumerable.ToList(x.genres.Split(','))
        });

    }

    public async Task<bool> UpdateAsync(Movie movie, CancellationToken token = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(token);
        using var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(new CommandDefinition("""
            delete from genres where movieid = @id
            """, new { id = movie.Id }, cancellationToken: token));

        foreach (var genre in movie.Genres)
        {
            await connection.ExecuteAsync(new CommandDefinition("""
                    insert into genres (movieId, name) 
                    values (@MovieId, @Name)
                    """, new { MovieId = movie.Id, Name = genre }));
        }

        var result = await connection.ExecuteAsync(new CommandDefinition("""
            update movies set slug = @Slug, title = @Title, yearofrelease = @YearOfRelease 
            where id = @Id
            """, movie, cancellationToken: token));

        transaction.Commit();
        return result > 0;

    }

    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(token);
        using var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(new CommandDefinition("""
            delete from genres where movieid = @id
            """, new { id }, cancellationToken: token));

        var result = await connection.ExecuteAsync(new CommandDefinition("""
            delete from movies where id = @id
            """, new { id }, cancellationToken: token));

        transaction.Commit();
        return result > 0;

    }

    public async Task<Movie?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken token = default)
    {
        using var conn = await connectionFactory.CreateConnectionAsync(token);
        var movie = await conn.QuerySingleOrDefaultAsync<Movie>(
              new CommandDefinition("""
            select m.*, round(avg(r.rating), 1) as rating, myr.rating as userrating
            from movies m
            left join ratings r on m.id = r.movieid
            left join ratings myr on m.id = myr.movieid
                and myr.userid = @userId
            where slug = @slug
            group by id, userrating
            """, new { slug, userId }, cancellationToken: token));

        if (movie is null)
            return null;

        var genres = await conn.QueryAsync<string>(
            new CommandDefinition(""""
            select name from genres
            where movieId = @Id            
            """", new { Id = movie.Id }, cancellationToken: token));

        foreach (var item in genres)
        {
            movie.Genres.Add(item);
        }

        return movie;
    }
    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default)
    {
        using var conn = await connectionFactory.CreateConnectionAsync(token);
        var total = await conn.ExecuteScalarAsync<int>(
          new CommandDefinition(""""
            select Count(*) from movies
            where id = @Id            
            """", new { Id = id }, cancellationToken: token));

        return total > 0;
    }

    public async Task<int> GetCountByAsync(string? title, int? year, CancellationToken token  = default)
    {
        using var conn = await connectionFactory.CreateConnectionAsync(token);
        var total = await conn.ExecuteScalarAsync<int>(
          new CommandDefinition(""""
            select Count(*) from movies m
            where
             (@title is null or m.title like ('%' || @title || '%'))
             and (@year is null or m.yearofrelease = @year) 
            """", new { title, year }, cancellationToken: token));

        return total;
    }
}