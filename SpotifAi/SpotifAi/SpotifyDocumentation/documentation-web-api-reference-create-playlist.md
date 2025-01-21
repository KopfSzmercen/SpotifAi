<output>
    <route_params>
        <param>
            <name>user_id</name>
            <type>string</type>
            <description>The user's Spotify user ID.</description>
            <example>smedjan</example>
        </param>
    </route_params>

    **<request_body_params>
        <param>
            <name>name</name>
            <type>string</type>
            <description>The name for the new playlist; this does not need to be unique.</description>
            <example>Your Coolest Playlist</example>
        </param>
        <param>
            <name>public</name>
            <type>boolean</type>
            <description>The playlist's public/private status; defaults to true (public).</description>
            <example>true</example>
        </param>
        <param>
            <name>collaborative</name>
            <type>boolean</type>
            <description>If true, the playlist will be collaborative; defaults to false. To create a collaborative playlist, public must also be set to false.</description>
            <example>false</example>
        </param>
        <param>
            <name>description</name>
            <type>string</type>
            <description>Value for playlist description as displayed in Spotify Clients and in the Web API.</description>
            <example>This is my favorite playlist.</example>
        </param>
    </request_body_params>
    
    <request_sample>
        POST /users/smedjan/playlists
        {
            "name": "Your Coolest Playlist",
            "public": true,
            "collaborative": false,
            "description": "A description for my playlist."
        }
    </request_sample>

</output>