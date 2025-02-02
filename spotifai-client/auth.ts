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

        const signInResponse = await fetch(
          `${backendRootUrl}/users/sign-in-jwt`,
          {
            method: "POST",
            body: JSON.stringify({
              email: parsedCredentials.data?.email,
              password: parsedCredentials.data?.password
            }),
            headers: {
              "Content-Type": "application/json"
            }
          }
        );

        if (!signInResponse.ok) return null;

        const signInResult = (await signInResponse.json()) as { token: string };

        const getMeResponse = await fetch(`${backendRootUrl}/users/me`, {
          headers: {
            Authorization: `Bearer ${signInResult.token}`
          }
        });

        const getMeResult = await getMeResponse.json();

        if (!getMeResponse.ok) return null;

        return {
          email: getMeResult.email,
          username: getMeResult.username,
          token: signInResult.token
        };
      }
    })
  ],
  callbacks: {
    authorized: async ({ auth }) => {
      // Logged in users are authenticated, otherwise redirect to login page
      return !!auth;
    },
    session: async ({ session, token }) => {
      // Add property to session, so it's available in the client
      session.user.username = token.username as string;
      session.user.token = token.token as string;
      session.user.email = token.email as string;
      return session;
    },
    jwt: async ({ token, user }) => {
      // Add property to token, so it's available in the client
      if (user) {
        token.username = user.username;
        token.email = user.email;
        token.token = user.token;
      }
      return token;
    }
  }
});
