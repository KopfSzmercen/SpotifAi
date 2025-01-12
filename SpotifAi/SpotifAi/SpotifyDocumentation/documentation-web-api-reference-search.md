<output>
    <query_params>
        <param>
            <name>q</name>
            <type>string</type>
            <description>Your search query. You can narrow down your search using field filters such as `album`, `artist`, `track`, `year`, `upc`, `tag:hipster`, `tag:new`, `isrc`, and `genre`.</description>
            <example>q=remaster%2520track%3ADoxy%2520artist%3AMiles%2520Davis</example>
        </param>
        <param>
            <name>type</name>
            <type>array of strings</type>
            <description>A comma-separated list of item types to search across. Allowed values: "album", "artist", "playlist", "track", "show", "episode", "audiobook".</description>
            <example>type=album,track</example>
        </param>
        <param>
            <name>market</name>
            <type>string</type>
            <description>An ISO 3166-1 alpha-2 country code. If a country code is specified, only content that is available in that market will be returned.</description>
            <example>market=ES</example>
        </param>
        <param>
            <name>limit</name>
            <type>integer</type>
            <description>The maximum number of results to return in each item type. Default: 20. Range: 0 - 50.</description>
            <example>limit=10</example>
        </param>
        <param>
            <name>offset</name>
            <type>integer</type>
            <description>The index of the first result to return. Use with limit to get the next page of search results. Default: 0. Range: 0 - 1000.</description>
            <example>offset=5</example>
        </param>
        <param>
            <name>include_external</name>
            <type>string</type>
            <description>If `include_external=audio` is specified it signals that the client can play externally hosted audio content.</description>
            <example>include_external=audio</example>
        </param>
    </query_params>
</output>