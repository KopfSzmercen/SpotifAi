"use client";

import { Button } from "@/components/ui/button";
import { signOut } from "next-auth/react";

const SignOutButton = () => {
  return (
    <Button
      onClick={async () => {
        await signOut({ redirect: true, redirectTo: "/login" });
      }}
    >
      Sign Out
    </Button>
  );
};

export default SignOutButton;
