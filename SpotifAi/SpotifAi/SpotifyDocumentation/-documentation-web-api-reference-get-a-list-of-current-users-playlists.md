<output>
    <query_params>
        <param>
            <name>limit</name>
            <type>integer</type>
            <description>The maximum number of items to return. Default: 20. Minimum: 1. Maximum: 50.</description>
            <example>limit=10</example>
        </param>
        <param>
            <name>offset</name>
            <type>integer</type>
            <description>The index of the first playlist to return. Default: 0 (the first object). Maximum offset: 100,000. Use with limit to get the next set of playlists.</description>
            <example>offset=5</example>
        </param>
    </query_params>
    
    <request_sample>
        https://api.spotify.com/v1/me/playlists?limit=10&offset=5
    </request_sample>
</output>