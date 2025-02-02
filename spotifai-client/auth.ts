import NextAuth from "next-auth";
import Credentials from "next-auth/providers/credentials";
import { authConfig } from "./auth.config";
import { z } from "zod";

export const { auth, signIn, signOut, handlers } = NextAuth({
  ...authConfig,
  providers: [
    Credentials({
      name: "credentials",
      type: "credentials",
      credentials: {
        email: { label: "email", type: "text" },
        password: { label: "password", type: "password" }
      },

      async authorize(credentials) {
        const parsedCredentials = z
          .object({ email: z.string().email(), password: z.string().min(5) })
          .safeParse(credentials);

        if (!parsedCredentials.success) {
          return null;
        }

        const backendRootUrl = process.env.BACKEND_ROOT_URL!;

        const response = await fetch(`${backendRootUrl}/users/sign-in`, {
          method: "POST",
          body: JSON.stringify({
            email: parsedCredentials.data?.email,
            password: parsedCredentials.data?.password
          }),
          headers: {
            "Content-Type": "application/json"
          }
        });

        if (!response.ok) return null;

        const getMeResponse = await fetch(`${backendRootUrl}/users/me`, {
          headers: {
            cookie: response.headers.get("set-cookie")!
          }
        });

        const responseBody = await getMeResponse.json();

        if (!getMeResponse.ok) return null;

        return {
          email: responseBody.email,
          username: responseBody.username,
          cookie: response.headers.get("set-cookie")!
        };
      }
    })
  ],
  callbacks: {
    authorized: async ({ auth }) => {
      // Logged in users are authenticated, otherwise redirect to login page
      return !!auth;
    },
    session: async ({ session, token, user }) => {
      // Add property to session, so it's available in the client
      session.user.username = token.username;
      session.user.cookie = token.cookie;
      return session;
    },
    jwt: async ({ token, user }) => {
      // Add property to token, so it's available in the client
      if (user) {
        token.username = user.username;
        token.cookie = user.cookie;
      }
      return token;
    }
  }
});
