namespace GuldeLib.Builders
{
    public class GameBuilder : Builder<Game>
    {
        public GameBuilder WithSceneName(string sceneName)
        {
            Object.SceneName = sceneName;
            return this;
        }
    }
}