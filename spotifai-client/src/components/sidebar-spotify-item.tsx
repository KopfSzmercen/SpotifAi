"use client";

import { Button } from "@/components/ui/button";
import { useSession } from "next-auth/react";
import { useEffect, useState } from "react";

const SidebarSpotifyItem = () => {
  const { data: session } = useSession();

  async function getMe(token: string): Promise<{
    email: string;
    username: string;
    connectedToSpotify: boolean;
  }> {
    const response = await fetch(
      process.env.NEXT_PUBLIC_BACKEND_ROOT_URL + "/users/me",
      {
        headers: {
          Authorization: `Bearer ${token}`
        }
      }
    );

    if (!response.ok) {
      throw new Error("Failed to fetch user data");
    }

    return response.json();
  }

  const connectToSpotify = async () => {
    const response = await fetch(
      process.env.NEXT_PUBLIC_BACKEND_ROOT_URL +
        "/spotify/initialize-authorization",
      {
        method: "GET",
        headers: {
          Authorization: `Bearer ${session!.user.token}`
        },
        mode: "no-cors"
      }
    );

    if (!response.ok) throw new Error("Failed to connect to Spotify");
    window.location.href = await response.text();
  };

  const [isConnected, setIsConnected] = useState(false);

  useEffect(() => {
    if (session?.user.token) {
      getMe(session.user.token).then((data) => {
        setIsConnected(data.connectedToSpotify);
      });
    }
  }, [session?.user.token]);

  return (
    <div>
      {isConnected ? (
        <span className="text-primary font-bold">Connected to Spotify</span>
      ) : (
        <Button onClick={connectToSpotify}>Connect to Spotify</Button>
      )}
    </div>
  );
};

export default SidebarSpotifyItem;
