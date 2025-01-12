<output>
    <request_body_params>
        <param>
            <name>device_id</name>
            <type>string</type>
            <description>The id of the device this command is targeting. If not supplied, the user's currently active device is the target.</description>
            <example>device_id=0d1841b0976bae2a3a310dd74c0f3df354899bc8</example>
        </param>
    </request_body_params>
    
    <request_sample>
        POST https://api.spotify.com/v1/me/player/next
        {
            "device_id": "0d1841b0976bae2a3a310dd74c0f3df354899bc8"
        }
    </request_sample>
</output>