import type { NextAuthConfig } from "next-auth";

export const authConfig = {
  pages: {
    signIn: "/login",
    signOut: "/"
  },
  callbacks: {
    authorized({ auth }) {
      const isAuthenticated = !!auth?.user;

      return isAuthenticated;
    }
  },
  providers: [],
  debug: true
} satisfies NextAuthConfig;
