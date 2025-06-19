# movies-api
my movies api for learning rest api development in asp.net core using web api


# general migration setups.
1. create entity model.
2. Do configuration inside Configuration directory using IEntityTypeConfiguration<Movie>
3. In configuration, we could also seed the database here.
4. Then run migrations add AddMovieTable 
5. Then database update
