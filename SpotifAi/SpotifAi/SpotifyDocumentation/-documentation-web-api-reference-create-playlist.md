<output>
    <route_params>
        <param>
            <name>user_id</name>
            <type>string</type>
            <description>The user's Spotify user ID.</description>
            <example>smedjan</example>
        </param>
    </route_params>

    <request_body_params>
        <param>
            <name>name</name>
            <type>string</type>
            <description>The name for the new playlist. This name does not need to be unique; a user may have several playlists with the same name.</description>
            <example>"Your Coolest Playlist"</example>
        </param>
        <param>
            <name>public</name>
            <type>boolean</type>
            <description>The playlist's public/private status: `true` the playlist will be public, `false` the playlist will be private. To create private playlists, the user must have granted the `playlist-modify-private` scope.</description>
            <example>true</example>
        </param>
        <param>
            <name>collaborative</name>
            <type>boolean</type>
            <description>If `true`, the playlist will be collaborative. To create a collaborative playlist, you must also set `public` to `false` and granted `playlist-modify-private` and `playlist-modify-public` scopes.</description>
            <example>false</example>
        </param>
        <param>
            <name>description</name>
            <type>string</type>
            <description>Value for playlist description as displayed in Spotify Clients and in the Web API.</description>
            <example>"A playlist for my favorite songs."</example>
        </param>
    </request_body_params>

    <request_sample>
        POST https://api.spotify.com/v1/users/smedjan/playlists
        {
            "name": "Your Coolest Playlist",
            "public": true,
            "collaborative": false,
            "description": "A playlist for my favorite songs."
        }
    </request_sample>
</output>