import { GalleryVerticalEnd } from "lucide-react";
import * as React from "react";

import SignOutButton from "@/components/auth/sign-out-button";
import { Button } from "@/components/ui/button";
import {
  Sidebar,
  SidebarContent,
  SidebarGroup,
  SidebarHeader,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
  SidebarRail
} from "@/components/ui/sidebar";

const isConnected = false;

export async function AppSidebar({
  ...props
}: React.ComponentProps<typeof Sidebar>) {
  return (
    <Sidebar {...props}>
      <SidebarHeader>
        <SidebarMenu>
          <SidebarMenuItem>
            <SidebarMenuButton size="lg" asChild>
              <a href="#">
                <div className="flex aspect-square size-8 items-center justify-center rounded-lg bg-sidebar-primary text-sidebar-primary-foreground">
                  <GalleryVerticalEnd className="size-4" />
                </div>
                <div className="flex flex-col gap-0.5 leading-none">
                  <span className="font-semibold">SpotifAi</span>
                </div>
              </a>
            </SidebarMenuButton>
          </SidebarMenuItem>
          <SidebarMenuItem className="">
            {isConnected ? <span>Connected</span> : <Button>Connect</Button>}
          </SidebarMenuItem>
        </SidebarMenu>
      </SidebarHeader>
      <SidebarContent>
        <SidebarGroup>
          <SidebarMenu></SidebarMenu>
        </SidebarGroup>
      </SidebarContent>
      <SidebarRail />
      <div className="flex flex-col items-center justify-center p-4">
        <SignOutButton />
      </div>
    </Sidebar>
  );
}
