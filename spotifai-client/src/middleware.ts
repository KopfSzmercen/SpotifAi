import NextAuth from "next-auth";
import { authConfig } from "../auth.config";

const PUBLIC_ROUTES = ["/login", "/register"];
const { auth } = NextAuth(authConfig);

export default auth((req) => {
  const { nextUrl } = req;

  const isAuthenticated = !!req.auth;
  const isPublicRoute = PUBLIC_ROUTES.includes(nextUrl.pathname);

  if (!isAuthenticated && !isPublicRoute)
    return Response.redirect(new URL("/login", nextUrl));
});

export const config = {
  matcher: ["/((?!api|_next/static|_next/image|favicon.ico).*)"]
};
