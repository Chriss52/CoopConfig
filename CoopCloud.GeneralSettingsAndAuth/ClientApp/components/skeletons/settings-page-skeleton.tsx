import React from "react";
import {
  Card,
  Skeleton,
  CardHeader,
  CardFooter,
  CardContent,
} from "@nubeteck/shadcn";

const SettingsPageSkeleton = () => {
  return (
    <>
      <Card className="shadow-none mb-4">
        <CardHeader className="space-y-2">
          <Skeleton className="h-6 w-64" />
          <Skeleton className="h-4 w-96" />
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-2 gap-6">
            <div className="space-y-2">
              <Skeleton className="h-4 w-32" />
              <Skeleton className="h-10 w-full" />
              <Skeleton className="h-3 w-64" />
            </div>
            <div className="space-y-2">
              <Skeleton className="h-4 w-40" />
              <Skeleton className="h-10 w-full" />
              <Skeleton className="h-3 w-72" />
            </div>
            <div className="space-y-2">
              <Skeleton className="h-4 w-36" />
              <Skeleton className="h-10 w-full" />
              <Skeleton className="h-3 w-56" />
            </div>
            <div className="flex items-center justify-between rounded-lg border p-4">
              <div className="space-y-1">
                <Skeleton className="h-4 w-48" />
                <Skeleton className="h-3 w-64" />
              </div>
              <Skeleton className="h-6 w-11 rounded-full" />
            </div>
          </div>
        </CardContent>
        <CardFooter className="flex justify-end">
          <Skeleton className="h-10 w-40" />
        </CardFooter>
      </Card>
      <Skeleton className="h-50 w-full" />
    </>
  );
};

export default React.memo(SettingsPageSkeleton);
