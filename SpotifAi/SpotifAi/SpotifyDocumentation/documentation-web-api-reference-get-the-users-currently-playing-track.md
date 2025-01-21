<output>
    <query_params>
        <param>
            <name>market</name>
            <type>string</type>
            <description>An ISO 3166-1 alpha-2 country code. If a country code is specified, only content that is available in that market will be returned.</description>
            <example>market=ES</example>
        </param>
        <param>
            <name>additional_types</name>
            <type>string</type>
            <description>A comma-separated list of item types that your client supports besides the default track type. Valid types are: track and episode.</description>
        </param>
    </query_params>

    <request_sample>
        https://api.spotify.com/v1/me/player/currently-playing?market=ES&additional_types=track,episode
    </request_sample>
</output>